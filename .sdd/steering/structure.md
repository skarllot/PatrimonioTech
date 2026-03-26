# Project Structure

## Organization Philosophy

Feature-first within each layer. Code is grouped by domain concept (e.g., `Credentials`, `Ativos`, `TiposAtivos`) inside each project, not by technical role.

## Layer Projects

| Project | Role |
|---|---|
| `PatrimonioTech.Domain` | Entities, value objects, domain services interfaces, no external dependencies |
| `PatrimonioTech.App` | Use cases (application services), request/response DTOs, app-level interfaces |
| `PatrimonioTech.Infra` | Repository implementations, LiteDB models, mappers, PBKDF2 key derivation |
| `PatrimonioTech.Gui` | Avalonia Views + ReactiveUI ViewModels, feature-foldered |
| `PatrimonioTech.Gui.Desktop` | Entry point: Jab container wiring, `Program.cs`, platform-specific startup |

## Directory Patterns

### Domain: Value Objects (`Domain/{Feature}/`)
Value objects have private constructors with a static `Create()` factory returning `Result<T, TError>`.
Partial classes decorated with `[Equatable]` for source-generated equality.
Example: `Domain/Common/ValueObjects/Cnpj.cs` + `Cnpj.Parser.cs`

### App: Use Cases (`App/{Feature}/v{N}/{Action}/`)
Versioned use cases with co-located Request, Response, and Error types.
Each use case exposes an `I{Name}UseCase` interface.
Example: `App/Credentials/v1/GetUserInfo/` contains `CredentialGetUserInfoRequest.cs`, `CredentialGetUserInfoResponse.cs`, `CredentialGetUserInfoError.cs`

### Infra: Implementations (`Infra/{Feature}/`)
Repositories implement App-layer interfaces. LiteDB models are separate from domain entities; Mapperly mappers convert between them.
Example: `Infra/Credentials/Services/FileUserCredentialRepository.cs` + `UserCredentialModel.cs` + `UserCredentialModelMapper.cs`

### Gui: Feature Folders (`Gui/{Feature}/`)
Each feature folder contains a paired View (`.axaml` + `.axaml.cs`) and ViewModel.
ViewModels inherit `RoutableViewModelBase` (which extends ReactiveUI's `ReactiveObject`).
`[Notify]` decorates backing fields for source-generated `INotifyPropertyChanged`.
Example: `Gui/Login/LoginView.axaml` + `LoginView.axaml.cs` + `LoginViewModel.cs`

### DI Modules (`{Layer}/DependencyInjection/`)
Each layer exposes a Jab module interface (e.g., `IDomainModule`, `IAppModule`).
`DesktopContainer` in `Gui.Desktop` imports all modules via `[Import<IXxxModule>]`.
`IFactory<T>` is used to create ViewModels on demand without exposing the container.

## Naming Conventions

- **Projects**: `PatrimonioTech.{Layer}` (PascalCase layer name)
- **Namespaces**: Mirror project + folder structure (e.g., `PatrimonioTech.App.Credentials.v1.GetUserInfo`)
- **Value Objects**: Named by domain concept (`Cnpj`, `B3Ticker`, `Password`)
- **Use Case DTOs**: `{Domain}{Action}Request/Response/Error` (e.g., `CredentialGetUserInfoRequest`)
- **Errors**: Union-like discriminated types or enums named `{Concept}Error` (e.g., `CnpjError`, `GetKeyError`)
- **ViewModels**: `{Feature}ViewModel` paired with `{Feature}View.axaml`

## Code Organization Principles

- Architecture tests (`tests/PatrimonioTech.Gui.Desktop.Tests/Architecture/`) enforce layer dependency rules at CI time
- View and ViewModel must reside in the same namespace (enforced by `ViewAndViewModelSameNamespaceRule`)
- Jab DI attributes restricted to `DependencyInjection` namespaces — other code must not reference Jab directly

---
_Document patterns, not file trees. New files following patterns shouldn't require updates_
