using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PatrimonioTech.App.Credentials.v1.GetUserInfo;
using PatrimonioTech.App.Credentials.v1.GetUsers;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Gui.Common;
using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Users.Create;
using PropertyChanged.SourceGenerator;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PatrimonioTech.Gui.Login;

public partial class LoginViewModel : RoutableViewModelBase
{
    // Source Streams
    [Notify] private string _usuario = string.Empty;
    [Notify] private string _senha = string.Empty;

    // Intermediate Streams
    private readonly Subject<bool> _canLoginSubject = new();
    private readonly Subject<ImmutableList<string>> _existingUsers = new();

    // View Model Factories
    private readonly IFactory<DashboardViewModel> _dashboard;

    // Use Cases
    private readonly Func<IObservable<ImmutableList<string>>> _getAllUserCredentials;

    private readonly Func<
        (string userName, string password),
        IObservable<Result<CredentialGetUserInfoResponse, CredentialGetUserInfoError>>
    > _getUserInfo;

    // Commands
    public ReactiveCommand<Unit, (string Usuario, string Senha)> Enter { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> CreateNew { get; }

    // Presentation Streams
    public IObservable<ImmutableList<string>> UsuariosExistentes => _existingUsers;

    public LoginViewModel(
        IScreen hostScreen,
        ICredentialGetUsersUseCase credentialGetUsersUseCase,
        ICredentialGetUserInfoUseCase credentialGetUserInfoUseCase,
        IFactory<UserCreateViewModel> userCreate,
        IFactory<DashboardViewModel> dashboard)
        : base(hostScreen)
    {
        // VIEW MODEL FACTORIES
        _dashboard = dashboard;

        // USE CASES
        _getAllUserCredentials = () => Observable
            .FromAsync(credentialGetUsersUseCase.Execute)
            .Select(x => x.UserNames);

        _getUserInfo = r => Observable
            .Return(new CredentialGetUserInfoRequest(r.userName, r.password))
            .SelectMany(credentialGetUserInfoUseCase.Execute);

        // ENTER COMMAND
        Enter = ReactiveCommand.Create(
            () => (Usuario, Senha),
            _canLoginSubject);

        // CREATE NEW COMMAND
        CreateNew = ReactiveCommand.CreateFromObservable(
            () => hostScreen.Router.Navigate.Execute(userCreate.Create()));

        this.WhenActivated(OnViewActivated);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _canLoginSubject.Dispose();
            _existingUsers.Dispose();
        }

        base.Dispose(disposing);
    }

    private void OnViewActivated(CompositeDisposable disposable)
    {
        // USER INPUT
        var whenUserChanged = this.WhenAnyValue(vm => vm.Usuario);

        var isUserSelected = whenUserChanged
            .CombineLatest(_getAllUserCredentials())
            .Select(x => x.Second.Contains(x.First, StringComparer.OrdinalIgnoreCase));

        // PASSWORD INPUT
        var whenPasswordChanged = this.WhenAnyValue(vm => vm.Senha);

        var isPasswordValid = whenPasswordChanged
            .Select(x => x.Length >= Password.PasswordMinLength);

        // ENTER COMMAND
        Observable
            .CombineLatest(isUserSelected, isPasswordValid, (v1, v2) => v1 && v2)
            .DistinctUntilChanged()
            .Subscribe(_canLoginSubject, disposable);

        // USER LIST OUTPUT
        _getAllUserCredentials().Subscribe(_existingUsers, disposable);

        // VALIDATIONS
        var userInfos = Enter.SelectMany(_getUserInfo);

        var passwordValidation = userInfos
            .Select(
                x => x.Case() is not CredentialGetUserInfoError.CryptographyError
                {
                    Error: GetKeyError.InvalidPassword
                })
            .StartWith(true);

        var userValidation = userInfos
            .Select(x => x.Case() is not CredentialGetUserInfoError.UserNotFound)
            .StartWith(true);

        userInfos.Successes()
            .SwitchSelect(_ => HostScreen.Router.Navigate.Execute(_dashboard.Create()))
            .Subscribe(disposable);

        this.ValidationRule(vm => vm.Senha, passwordValidation, "Senha inválida").DisposeWith(disposable);
        this.ValidationRule(vm => vm.Senha, isPasswordValid, "Senha muito curta").DisposeWith(disposable);

        this.ValidationRule(vm => vm.Usuario, userValidation, "Usuário não encontrado").DisposeWith(disposable);
    }
}
