using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.App.Credentials.v1.GetUserAvailability;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Gui.Common;
using PropertyChanged.SourceGenerator;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PatrimonioTech.Gui.Users.Create;

public sealed partial class UserCreateViewModel : RoutableViewModelBase
{
    // Source Streams
    [Notify] private string _userName = string.Empty;
    [Notify] private string _password = string.Empty;
    [Notify] private string _passwordConfirmation = string.Empty;

    // Intermediate Streams
    private readonly Subject<bool> _canCreateSubject = new();
    private readonly ObservableAsPropertyHelper<bool> _isCreating;

    // Use Cases
    private readonly IUserGetAvailabilityUseCase _getUserAvailabilityUseCase;

    // Commands
    public ReactiveCommand<Unit, IRoutableViewModel> Cancel { get; }
    public ReactiveCommand<Unit, Result<Unit, CredentialAddUserError>> Create { get; }

    // Interactions
    public Interaction<string, Unit> ShowError { get; } = new();

    // Bindable Properties
    public bool IsCreating => _isCreating.Value;

    public UserCreateViewModel(
        IScreen hostScreen,
        IUserGetAvailabilityUseCase getUserAvailabilityUseCase,
        ICredentialAddUserUseCase addUserUseCase)
        : base(hostScreen)
    {
        // USE CASES
        _getUserAvailabilityUseCase = getUserAvailabilityUseCase;

        // CANCEL COMMAND
        Cancel = ReactiveCommand.CreateFromObservable(() => hostScreen.Router.NavigateBack.Execute());

        // CREATE COMMAND
        Create = ReactiveCommand.CreateFromObservable(
            (Unit _) => Observable
                .Return(new CredentialAddUserRequest(UserName, Password))
                .SelectMany(addUserUseCase.Execute),
            _canCreateSubject);

        _isCreating = Create.IsExecuting.ToProperty(this, vm => vm.IsCreating);

        this.WhenActivated(OnViewActivated);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _canCreateSubject.Dispose();
            _isCreating.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnViewActivated(CompositeDisposable disposable)
    {
        // USERNAME INPUT
        var whenUserNameChanged = this.WhenAnyValue(vm => vm.UserName);

        var isUserValid = whenUserNameChanged
            .Select(v => !v.AsSpan().Trim().IsWhiteSpace());

        var isUserLongEnough = whenUserNameChanged
            .Select(v => v.Trim().Length >= AddUserScenario.NameMinLength);

        var isUserAvailable = whenUserNameChanged
            .Select(name => Observable.FromAsync(ct => _getUserAvailabilityUseCase.Execute(new UserGetAvailabilityRequest(name), ct)))
            .Switch()
            .Select(r => !r.Exists);

        // PASSWORD INPUT
        var whenPasswordChanged = this.WhenAnyValue(vm => vm.Password);

        var isPasswordValid = whenPasswordChanged
            .Select(x => x.Length >= Domain.Credentials.Password.PasswordMinLength);

        // PASSWORD CONFIRMATION INPUT
        var isConfirmationMatch = Observable
            .CombineLatest(
                whenPasswordChanged,
                this.WhenAnyValue(vm => vm.PasswordConfirmation),
                (v1, v2) => string.Equals(v1, v2, StringComparison.Ordinal));

        // CREATE COMMAND
        Observable
            .CombineLatest(
                isUserValid,
                isUserLongEnough,
                isPasswordValid,
                isConfirmationMatch,
                isUserAvailable,
                (v1, v2, v3, v4, v5) => v1 && v2 && v3 && v4 && v5)
            .DistinctUntilChanged()
            .Subscribe(_canCreateSubject, disposable);

        // NAVIGATION
        Create.Successes()
            .SelectMany(_ => HostScreen.Router.NavigateBack.Execute())
            .Subscribe(disposable);

        // ERROR HANDLING
        Create.Failures()
            .Select(MapErrorToMessage)
            .SelectMany(msg => ShowError.Handle(msg))
            .Subscribe(disposable);

        // VALIDATIONS
        this.ValidationRule(vm => vm.UserName, isUserAvailable, "Usuário já existe").DisposeWith(disposable);
        this.ValidationRule(vm => vm.UserName, isUserLongEnough, "Nome muito curto").DisposeWith(disposable);
        this.ValidationRule(vm => vm.Password, isPasswordValid, "Senha muito curta").DisposeWith(disposable);
        this.ValidationRule(vm => vm.PasswordConfirmation, isConfirmationMatch, "As senhas não coincidem")
            .DisposeWith(disposable);
    }

    private static string MapErrorToMessage(CredentialAddUserError error) =>
        error switch
        {
            CredentialAddUserError.BusinessError { Error: AddUserCredentialError.KeyDerivationFailed }
                => "Falha na proteção dos dados. Tente novamente.",
            CredentialAddUserError.BusinessError
                => "Dados inválidos. Verifique os campos e tente novamente.",
            CredentialAddUserError.StorageError
                => "O nome de usuário já foi registrado. Escolha outro nome.",
            CredentialAddUserError.DatabaseError
                => "Não foi possível criar o banco de dados. Verifique o espaço em disco e tente novamente.",
            _ => "Falha na proteção dos dados. Tente novamente."
        };
}
