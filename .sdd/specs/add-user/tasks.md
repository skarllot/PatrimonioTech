# Implementation Plan

- [x] 1. Verify and test Domain layer credential rules
- [x] 1.1 (P) Verify Password value object and add unit tests
  - Confirm the `Password` value object enforces: null/empty/whitespace → error, fewer than 8 characters → error, 8+ characters → success
  - Verify `PasswordMinLength` constant equals 8 and is publicly accessible for UI validation
  - Add unit tests covering all boundary cases: null, empty string, whitespace-only, 7-character, 8-character, and longer passwords
  - _Requirements: 4.2, 4.3_

- [x] 1.2 (P) Verify AddUserScenario and add unit tests
  - Confirm name validation rejects trimmed names shorter than 3 characters and accepts names with 3+ characters
  - Confirm password validation delegates to `Password.Create()` and propagates errors correctly
  - Confirm key derivation is called on valid inputs and failures are propagated as `KeyDerivationFailed`
  - Confirm successful execution produces `UserCredentialAdded` with correct `Name`, `PasswordHash` (from `IPhcString`), and a non-empty `DatabaseId`
  - Add unit tests with a mock `IKeyDerivation`: name too short → `NameTooShort`, invalid password → `InvalidPassword`, key derivation failure → `KeyDerivationFailed`, valid inputs → `Ok(UserCredentialAdded)`
  - Verify `NameMinLength` constant equals 3 and is publicly accessible
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 5.1, 5.2_

- [ ] 2. Verify and test App layer use cases
- [ ] 2.1 (P) Verify UserGetAvailabilityUseCase
  - Confirm the use case loads all credentials and performs case-insensitive locale-aware comparison (`CurrentCultureIgnoreCase`)
  - Verify it returns `Exists = true` for matching usernames regardless of casing and `Exists = false` for unknown names
  - Add or verify unit tests covering: exact match, different-case match, no match
  - _Requirements: 3.1, 3.4_

- [ ] 2.2 (P) Verify CredentialAddUserUseCase orchestration
  - Confirm the pipeline order: domain scenario → key re-derivation → database provisioning → credential persistence
  - Verify that a business error from `AddUserScenario` short-circuits without calling database admin or repository
  - Verify that a database creation error short-circuits without persisting credentials (satisfying the rollback guarantee)
  - Verify that a storage error propagates correctly
  - Verify that on success the use case returns `Ok(Unit)`
  - Add or verify unit tests for each short-circuit path and the success path
  - _Requirements: 5.1, 5.2, 6.1, 6.2, 7.1, 7.2_

- [ ] 3. Verify Infra layer implementations
- [ ] 3.1 (P) Verify Pbkdf2KeyDerivation and PhcString round-trip
  - Confirm `CreateKey` generates a valid `IPhcString` and returns `Ok`
  - Confirm `TryGetKey` with the correct password and hash returns the database key successfully
  - Confirm `TryGetKey` with an incorrect password returns an error
  - Verify `Pbkdf2PhcString.Parse` handles valid and malformed PHC strings correctly
  - Add or verify integration tests for the full round-trip
  - _Requirements: 5.1, 5.2_

- [ ] 3.2 (P) Verify FileUserCredentialRepository
  - Confirm `Add` persists a credential to the JSON file and can be retrieved via `GetAll`
  - Confirm `Add` with a duplicate name (case-insensitive) returns `NameAlreadyExists`
  - Verify atomic write strategy (temp file + `File.Replace`) is implemented
  - Add or verify integration tests for first add, duplicate add, and round-trip read
  - _Requirements: 7.1, 7.2_

- [ ] 3.3 (P) Verify LiteDbDatabaseAdmin
  - Confirm `CreateDatabase` creates an encrypted LiteDB file at the path derived from the database GUID
  - Confirm the database is accessible with the provided key
  - Add or verify an integration test for database creation
  - _Requirements: 6.1, 6.2_

- [ ] 4. Verify and test GUI layer registration form
- [ ] 4.1 Verify UserCreateView form layout
  - Confirm the view contains input fields for username, password, and password confirmation
  - Confirm the view has a Create button (labeled "Criar") and a Cancel button (labeled "Cancelar")
  - Confirm inputs are disabled and a spinner is shown inside the Create button while `IsCreating` is true
  - _Requirements: 1.1, 1.2_

- [ ] 4.2 Verify UserCreateViewModel validation rules and commands
  - Confirm five validation streams exist: `isUserValid` (non-whitespace), `isUserLongEnough` (trimmed ≥ 3), `isPasswordValid` (≥ 8 chars), `isConfirmationMatch` (ordinal equality), `isUserAvailable` (async check)
  - Confirm `CombineLatest` with `DistinctUntilChanged` drives the `_canCreateSubject` that gates the Create command
  - Confirm Create is disabled until first availability response arrives (no initial value on `_canCreateSubject`)
  - Verify four `ValidationRule` messages: "Usuário já existe", "Nome muito curto", "Senha muito curta", "As senhas não coincidem"
  - Verify Cancel command navigates back without creating a user
  - Verify Create command on success navigates back
  - Verify Create command on error triggers `ShowError` interaction with the correct Portuguese message per error variant
  - Verify `isUserAvailable` uses `Switch()` to cancel in-flight availability requests
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3_

- [ ] 4.3 Add UserCreateViewModel unit tests
  - Set up test infrastructure with mocked use cases and a test `IScreen` router
  - Test: empty/whitespace username → Create disabled
  - Test: username with fewer than 3 characters → "Nome muito curto" validation message and Create disabled
  - Test: password shorter than 8 characters → "Senha muito curta" validation message and Create disabled
  - Test: mismatched password confirmation → "As senhas não coincidem" validation message and Create disabled
  - Test: existing username → "Usuário já existe" validation message and Create disabled
  - Test: all fields valid and username available → Create enabled
  - Test: successful Create → navigates back
  - Test: failed Create → ShowError interaction triggered with correct message
  - _Requirements: 1.3, 1.4, 2.1, 2.2, 2.3, 2.4, 2.5, 3.2, 3.3_

- [ ] 5. Verify DI wiring and end-to-end integration
- [ ] 5.1 Verify dependency injection registration
  - Confirm all new services are registered in their respective Jab DI modules: `IAddUserScenario`, `IKeyDerivation`, `IUserCredentialRepository`, `IDatabaseAdmin`, `ICredentialAddUserUseCase`, `IUserGetAvailabilityUseCase`, `UserCreateViewModel`
  - Confirm `UserCreateViewModel` is registered as `Transient` for fresh instance per navigation
  - Confirm singleton and scoped lifetimes match design expectations
  - _Requirements: 4.4_

- [ ] 5.2 Verify architecture test compliance
  - Run existing architecture tests to confirm the add-user implementation does not violate layer dependency rules
  - Confirm Domain layer has no dependency on App, Infra, or Gui
  - Confirm Jab DI attributes are only used in `DependencyInjection` namespaces
  - _Requirements: 4.4_
