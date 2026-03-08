# Research & Design Decisions: Add User

---
**Purpose**: Capture discovery findings, architectural investigations, and rationale that inform the technical design.

---

## Summary

- **Feature**: `add-user`
- **Discovery Scope**: Extension — reverse-engineered from a fully existing implementation across all five projects
- **Key Findings**:
  - All components (Domain, App, Infra, Gui, DI) are already implemented; the design document reflects their actual structure and contracts
  - The feature follows every established project pattern without deviation: value objects, versioned use cases, `RoutableViewModelBase`, ReactiveUI.Validation, Jab DI
  - One notable risk identified: orphaned per-user LiteDB files can remain on disk if `CreateDatabase` succeeds but `Add` to the credential store subsequently fails — no cleanup mechanism exists

---

## Research Log

### Pattern Analysis: Existing Credentials Feature

- **Context**: The `add-user` spec was initialized after the implementation already existed; discovery required reading the codebase rather than consulting external sources.
- **Sources Consulted**: All source files under `src/PatrimonioTech.*/Credentials/`, `src/PatrimonioTech.Gui/Users/Create/`, DI modules, and steering documents.
- **Findings**:
  - Domain layer uses `[Union]` FxKit discriminated error unions for all error types (`AddUserCredentialError`, `CredentialAddUserError`)
  - `AddUserScenario` enforces: name ≥ 3 chars (after non-null/whitespace strip), password via `Password.Create` (≥ 8 chars, non-empty/whitespace), KeySize in [128, 4096], Iterations in [1000, 100,000,000]
  - `Password.PasswordMinLength = 8` is a public constant reused by the ViewModel for its reactive validation stream, ensuring UI and domain agree on the minimum
  - `UserGetAvailabilityUseCase` performs a full `GetAll` scan with `StringComparison.CurrentCultureIgnoreCase`; appropriate for a local store with an expected user count under 100
  - `UserCreateViewModel` uses a `Subject<bool> _canCreateSubject` pattern (also used by `LoginViewModel`) to gate `ReactiveCommand` execution without creating a separate `CanExecute` observable that could produce emissions before the view is activated
  - No debounce is applied to the username availability check stream — acceptable since `IUserCredentialRepository` is a local file store
- **Implications**:
  - Design must document the two-step key derivation: `CreateKey` (in domain scenario, produces Salt + EncryptedKey) vs. `TryGetKey` (in use case, re-derives runtime key for `CreateDatabase`)
  - The ViewModel consumes `Create` result but only acts on success; errors are not surfaced to the user in the current implementation

### Pattern Analysis: RoutableViewModelBase and Navigation

- **Context**: Understanding how navigation and lifecycle management work across the existing ViewModels.
- **Sources Consulted**: `RoutableViewModelBase.cs`, `LoginViewModel.cs`, `UserCreateViewModel.cs`
- **Findings**:
  - All ViewModels extend `RoutableViewModelBase`, which implements `IRoutableViewModel`, `INotifyDataErrorInfo`, `IValidatableViewModel`, and `IDisposable`
  - `IValidatableViewModel` from ReactiveUI.Validation bridges `ValidationContext` to Avalonia's `INotifyDataErrorInfo` via `ReactiveValidationSubject`
  - Validation rules (`this.ValidationRule(...)`) and subscriptions are set up inside `this.WhenActivated(OnViewActivated)` with a `CompositeDisposable`, ensuring proper cleanup on deactivation
  - Navigation back uses `HostScreen.Router.NavigateBack.Execute()` wrapped in `ReactiveCommand.CreateFromObservable`
- **Implications**:
  - Design must specify the activation/disposal pattern as a constraint on the ViewModel component

### Pattern Analysis: Jab DI Registration Scopes

- **Context**: Verifying DI lifecycle choices for each component.
- **Sources Consulted**: `IDomainModule.cs`, `IAppModule.cs`, `IInfraModule.cs`, `IGuiModule.cs`
- **Findings**:
  - `AddUserScenario`, `CredentialAddUserUseCase`, `UserGetAvailabilityUseCase` are registered as **Scoped**
  - `Pbkdf2KeyDerivation` is **Singleton** (stateless crypto service)
  - `LiteDbDatabaseAdmin` is **Singleton** (stateless file creator)
  - `FileUserCredentialRepository` is **Scoped** (likely due to potential per-request state or test isolation)
  - `UserCreateViewModel` is **Transient** (new instance per navigation)
- **Implications**:
  - Design must reflect these scopes to prevent incorrect registrations during maintenance

---

## Architecture Pattern Evaluation

| Option | Description | Strengths | Risks / Limitations | Notes |
|--------|-------------|-----------|---------------------|-------|
| Follow existing Clean Architecture | Extend Domain/App/Infra/Gui with Credentials feature folder | Zero friction, consistent with all existing patterns, enforced by NetArchTest | None for this feature size | Selected — all components already implemented this way |
| Separate CQRS handlers | Distinct command/query handler classes per use case | Explicit separation | Over-engineering for 2 use cases | Rejected — existing versioned use case pattern is sufficient |

---

## Design Decisions

### Decision: Two-Step Key Derivation in CredentialAddUserUseCase

- **Context**: The App use case must both store the key material (for future logins) and obtain the runtime encryption key (to lock the new database).
- **Alternatives Considered**:
  1. Store only the raw PBKDF2 output — simpler, but requires re-derivation at login time (already the case)
  2. Have `AddUserScenario` return both the event and the runtime key — leaks infrastructure concern into domain
- **Selected Approach**: `AddUserScenario.Execute` calls `IKeyDerivation.CreateKey` to produce `(Salt, EncryptedKey)` stored in `UserCredentialAdded`. The use case then calls `IKeyDerivation.TryGetKey` a second time with the same password to obtain the actual runtime key string passed to `CreateDatabase`.
- **Rationale**: Keeps the domain scenario focused on validation and event production; the use case owns the infrastructure orchestration.
- **Trade-offs**: Two PBKDF2 derivations occur during registration (minor performance cost, irrelevant at registration frequency).
- **Follow-up**: Verify that `TryGetKey` called immediately after `CreateKey` with the same inputs deterministically succeeds.

### Decision: Subject-Based CanExecute Gate for Create Command

- **Context**: The `Create` command must be disabled until all four async conditions are simultaneously true, including `isUserAvailable` which only emits after the first repository query.
- **Alternatives Considered**:
  1. Pass `IObservable<bool>` directly as `canExecute` to `ReactiveCommand.CreateFromObservable` — would enable the command before `isUserAvailable` emits its first value
  2. `Subject<bool>` seeded with `false` — naturally keeps the command disabled until `CombineLatest` emits
- **Selected Approach**: `Subject<bool> _canCreateSubject` driven by `CombineLatest(isUserValid, isPasswordValid, isConfirmationMatch, isUserAvailable)` with `DistinctUntilChanged`.
- **Rationale**: Matches the `LoginViewModel` pattern; avoids command being enabled prematurely.
- **Trade-offs**: `Subject` must be disposed explicitly (done in `Dispose(bool disposing)`).

### Decision: No Debounce on Username Availability Check

- **Context**: Availability check is triggered on every `UserName` change. Debouncing would reduce repository calls.
- **Selected Approach**: No debounce applied.
- **Rationale**: `FileUserCredentialRepository` is a local file read with no network latency; the performance cost is negligible.
- **Trade-offs**: Slightly higher I/O than necessary; acceptable for the expected use case.

---

## Risks & Mitigations

- **Orphaned database file on storage failure**: If `CreateDatabase` succeeds but `Add` to the credential store fails, the LiteDB file remains on disk. Mitigation: Acceptable for MVP; could be addressed in a future cleanup task by detecting unreferenced database files.
- **Race condition on username uniqueness**: If two registrations are submitted concurrently with the same username, both might pass the availability check before either writes. Mitigation: `IUserCredentialRepository.Add` enforces uniqueness at write time (`UserCredentialAddError.NameAlreadyExists`), providing the last line of defense.
- **No error feedback in UI on Create failure**: The current ViewModel silently absorbs `CredentialAddUserError` variants. Mitigation: Future work; out of scope for the current requirements.

---

## References

- `src/PatrimonioTech.Domain/Credentials/` — All domain types for the Credentials feature
- `src/PatrimonioTech.App/Credentials/v1/` — All application use cases for the Credentials feature
- `src/PatrimonioTech.Infra/Credentials/` — Infrastructure implementations
- `src/PatrimonioTech.Gui/Users/Create/` — GUI ViewModel and View for user registration
- `.sdd/steering/tech.md` — Technology stack and error-handling standards
- `.sdd/steering/structure.md` — Layer and naming conventions
