using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PatrimonioTech.App.Credentials.v1.GetUserInfo;
using PatrimonioTech.App.Credentials.v1.GetUsers;
using PatrimonioTech.Domain.Common;
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

    public ReactiveCommand<Unit, (string Usuario, string Senha)> Enter { get; }
    public ReactiveCommand<Unit, Unit> CreateNew { get; }

    // Presentation Streams
    public IObservable<ImmutableList<string>> UsuariosExistentes { get; }

    public LoginViewModel(
        IScreen hostScreen,
        ICredentialGetUsersUseCase credentialGetUsersUseCase,
        ICredentialGetUserInfoUseCase credentialGetUserInfoUseCase,
        IFactory<UserCreateViewModel> userCreate,
        IFactory<DashboardViewModel> dashboard)
        : base(hostScreen)
    {
        // USE CASES
        var allUserCredentials = Observable
            .FromAsync(credentialGetUsersUseCase.Execute)
            .Select(x => x.UserNames)
            .Replay(1)
            .RefCount();

        var getUserInfo = ((string userName, string password) r) => Observable
            .Return(new CredentialGetUserInfoRequest(r.userName, r.password))
            .SelectMany(credentialGetUserInfoUseCase.Execute)
            .Replay(1)
            .RefCount();

        // USER INPUT
        var whenUserChanged = this.WhenAnyValue(vm => vm.Usuario);

        var isUserSelected = whenUserChanged
            .CombineLatest(allUserCredentials)
            .Select(x => x.Second.Contains(x.First));

        // PASSWORD INPUT
        var whenPasswordChanged = this.WhenAnyValue(vm => vm.Senha);

        var isPasswordValid = whenPasswordChanged
            .Select(x => x.Length >= Password.PasswordMinLength);

        // ENTER COMMAND
        var canLogin = Observable
            .CombineLatest(isUserSelected, isPasswordValid, (v1, v2) => v1 && v2)
            .DistinctUntilChanged();

        Enter = ReactiveCommand.Create(
            () => (Usuario, Senha),
            canLogin);

        // CREATE NEW COMMAND
        CreateNew = ReactiveCommand.Create(
            () => { hostScreen.Router.Navigate.Execute(userCreate.Create()); });

        // USER LIST OUTPUT
        UsuariosExistentes = allUserCredentials;

        // VALIDATIONS
        var userInfos = Enter.SelectMany(getUserInfo);

        var passwordValidation = userInfos
            .Select(x => x.Case() is not CredentialGetUserInfoError { Value: GetKeyError.InvalidPassword })
            .StartWith(true);

        var userValidation = userInfos
            .Select(
                x => x.Case() is not CredentialGetUserInfoError
                {
                    Value: CredentialGetUserInfoError.Other.UserNotFound
                })
            .StartWith(true);

        this.WhenActivated(
            disposable =>
            {
                userInfos
                    .Successes()
                    .SwitchSelect(_ => hostScreen.Router.Navigate.Execute(dashboard.Create()))
                    .Subscribe(disposable);

                this.ValidationRule(vm => vm.Senha, passwordValidation, "Senha inválida").DisposeWith(disposable);
                this.ValidationRule(vm => vm.Senha, isPasswordValid, "Senha muito curta").DisposeWith(disposable);

                this.ValidationRule(vm => vm.Usuario, userValidation, "Usuário não encontrado").DisposeWith(disposable);
            });
    }
}
