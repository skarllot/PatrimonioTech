# Gap Analysis: Add User

## Analysis Summary

- **The add-user feature is already substantially implemented** across all four layers (Domain, App, Infra, GUI) with working use cases, domain scenarios, infrastructure services, and a complete UI form.
- All 7 requirements have corresponding implementation artifacts: domain validation, PBKDF2 key derivation, per-user LiteDB databases, file-based credential persistence, and reactive UI validation.
- Existing test coverage spans App-layer use cases and Infra-layer cryptography, but **no Domain-layer scenario tests or GUI ViewModel tests** exist.
- The gap is primarily in **test completeness and potential minor alignment issues** between requirements and implementation, not in missing capabilities.
- Implementation risk is **Low** — the feature follows all established codebase patterns.

---

## Requirement-to-Asset Map

| Requirement | Existing Assets | Gap Status |
|---|---|---|
| **R1: User Registration Form** | `UserCreateView.axaml` (username, password, confirmation fields, Create/Cancel buttons), `UserCreateViewModel.cs` (navigation commands) | **Covered** |
| **R2: Real-Time Input Validation** | `UserCreateViewModel.cs` — ReactiveUI.Validation rules for username non-empty, password ≥8 chars, confirmation match; `Subject<bool> _canCreateSubject` gates Create command | **Covered** |
| **R3: Real-Time Username Availability** | `UserGetAvailabilityUseCase` (case-insensitive check), `UserCreateViewModel.cs` async availability query, "Usuário já existe" message | **Covered** |
| **R4: Business Rules for Credentials** | `AddUserScenario.cs` — name ≥3 chars, password validation via `Password.Create()`; enforced independently of UI | **Covered** |
| **R5: Password-Based Data Protection** | `Pbkdf2KeyDerivation.cs` (PBKDF2-SHA512 + AES-CBC), `CredentialAddUserUseCase` orchestrates key derivation; failure → registration rejected | **Covered** |
| **R6: Per-User Data Isolation** | `LiteDbDatabaseAdmin.CreateDatabase()` provisions encrypted LiteDB per user (GUID filename); `CredentialAddUserUseCase` rolls back on failure | **Covered** |
| **R7: Credential Persistence** | `FileUserCredentialRepository` (JSON at `databases.json`), atomic writes, duplicate name rejection (OrdinalIgnoreCase) | **Covered** |

### Gaps Tagged

| Gap | Type | Detail |
|---|---|---|
| Domain scenario tests | **Missing** | `AddUserScenario` has no dedicated unit tests |
| ViewModel tests | **Missing** | `UserCreateViewModel` has no tests for validation rules or command behavior |
| R3 availability check debouncing | **Unknown** | Requirements don't specify debounce, but reactive pipeline may or may not throttle rapid keystrokes — verify during implementation review |
| R6 rollback on DB creation failure | **Constraint** | `CredentialAddUserUseCase` must not persist credentials if `CreateDatabase` fails — verify orchestration order (currently: scenario → DB create → credential add, which is correct) |

---

## Implementation Approach Options

### Option A: Extend Existing — Add Missing Tests Only

**Rationale**: All functional code exists. The only gaps are test coverage.

**What to do**:
- Add `AddUserScenarioTest.cs` in `tests/PatrimonioTech.Domain.Tests/Credentials/Actions/AddUser/`
- Add `UserCreateViewModelTest.cs` in `tests/PatrimonioTech.Gui.Tests/Users/Create/` (if GUI test project exists)
- Review and verify existing implementation matches all acceptance criteria precisely

**Trade-offs**:
- ✅ No new production code needed
- ✅ Minimal risk — tests only validate what exists
- ✅ Fast delivery
- ❌ May miss subtle requirement-implementation mismatches

### Option B: Comprehensive Review + Test + Fix

**Rationale**: Treat the gap analysis as validation — systematically verify each acceptance criterion against the implementation and fix any discrepancies found.

**What to do**:
- Audit each acceptance criterion against code behavior
- Add missing tests (Domain scenario + ViewModel)
- Fix any mismatches found (e.g., validation message text, edge cases)
- Ensure orchestration order in use case prevents partial state on failure

**Trade-offs**:
- ✅ Highest confidence in requirement alignment
- ✅ Catches edge cases and subtle bugs
- ❌ More effort than Option A
- ❌ May surface issues that require design decisions

### Option C: Validate via Spec Tasks (Recommended)

**Rationale**: Use the existing spec-driven workflow to generate tasks that systematically verify and test each requirement.

**What to do**:
- Proceed to `/kiro:spec-design` and `/kiro:spec-tasks` phases
- Tasks will be scoped to: verify existing code + add tests + fix gaps
- Implementation phase executes tasks with TDD methodology

**Trade-offs**:
- ✅ Structured, traceable verification
- ✅ Aligns with project's spec-driven workflow
- ✅ Tasks document what was verified
- ❌ More ceremony than Option A

---

## Implementation Complexity & Risk

- **Effort**: **S (1–3 days)** — Feature is already implemented; remaining work is testing and verification
- **Risk**: **Low** — All patterns are established, technology is familiar, scope is clear, no architectural changes needed

---

## Recommendations for Design Phase

### Preferred Approach
**Option C** — Proceed through the spec-driven workflow. The design document already exists (phase: `design-generated`). Generate tasks focused on verification and testing rather than greenfield implementation.

### Key Decisions
1. Whether to add ViewModel-level tests (requires test infrastructure for ReactiveUI — may need `TestScheduler` setup)
2. Whether the current username availability check needs debouncing/throttling for UX (not in requirements but good practice)

### Research Items
- **ViewModel testing infrastructure**: Verify if the project has ReactiveUI test helpers or if `Microsoft.Reactive.Testing` needs to be added
- **Domain test project**: Check if `PatrimonioTech.Domain.Tests` exists or needs to be created
