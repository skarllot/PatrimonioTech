# Technology Stack

## Architecture

Clean Architecture with strict layer separation enforced by architecture tests (NetArchTest):

```
Domain → App → Infra
                ↑
              Gui → Gui.Desktop (entry point)
```

Dependency rule: `Domain` has no dependencies on other layers; `App` depends only on `Domain`; `Infra` and `Gui` depend on `App`; `Jab` DI references are restricted to `DependencyInjection` namespaces only.

## Core Technologies

- **Language**: C# on .NET 10
- **UI Framework**: Avalonia 11 (cross-platform desktop: Windows, macOS, Linux)
- **Reactive**: ReactiveUI + System.Reactive (Rx.NET) for all ViewModel logic and data flow
- **DI**: Jab (compile-time source-generated IoC container)
- **Database**: LiteDB (embedded NoSQL, single-file local storage)

## Key Libraries

- **FxKit** `0.9.1` — Functional programming primitives: `Result<TValue, TError>`, `Option`, railway-oriented pipelines
- **Generator.Equals** — Source-generated `Equals`/`GetHashCode` via `[Equatable]` on partial classes
- **PropertyChanged.SourceGenerator** — `[Notify]` attribute for source-generated `INotifyPropertyChanged` in ViewModels
- **Riok.Mapperly** — Source-generated object mappers in Infra layer
- **ReactiveUI.Validation** — Inline validation rules on ViewModels (`this.ValidationRule(...)`)
- **Raiqub.Generators.EnumUtilities** — `[EnumGenerator]` for efficient enum utilities (descriptions, parsing)

## Development Standards

### Functional Error Handling
- Never throw exceptions for domain validation; return `Result<T, TError>` (FxKit)
- Value objects expose a static `Create(...)` factory that returns `Result`
- Error discrimination uses `result.Case()` extension + pattern matching, not exception catching

### Nullable Reference Types
- All projects use `<Nullable>enable</Nullable>` — null-safety is enforced at compile time

### Type Safety
- No use of `dynamic` or unchecked casts in domain/app layers
- Source generators preferred over reflection (DI, mappers, property notification, equality)

### Code Comments
- Avoid explanatory comments; code should be self-documenting
- Grouping comments (e.g. `// Commands`, `// Bindable Properties`) are allowed

### Testing
- **Unit**: xUnit + FluentAssertions + FxKit.Testing (`Should().BeOk()`, `Should().BeErr()`)
- **Architecture**: NetArchTest verifies layer dependencies and DI isolation

## Development Environment

### Required Tools
- .NET 10 SDK
- IDE with Roslyn source generator support (Visual Studio 2022 / Rider)

### Common Commands
```bash
# Build
dotnet build

# Test
dotnet test

# Run desktop app
dotnet run --project src/PatrimonioTech.Gui.Desktop
```

## Key Technical Decisions

- **Compile-time DI (Jab)** over runtime reflection for startup performance and AOT compatibility
- **LiteDB** chosen for single-file embedded storage — no external database process required
- **Avalonia compiled bindings** (`AvaloniaUseCompiledBindingsByDefault=true`) for performance and type safety in XAML
- **Source generators** over runtime reflection throughout (DI, mappers, equality, property change notification)

---
_Document standards and patterns, not every dependency_
