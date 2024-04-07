using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using PatrimonioTech.App.Credentials.v1.GetUserInfo;
using PatrimonioTech.App.Credentials.v1.GetUsers;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Gui.Common;
using PatrimonioTech.Gui.Dashboard;
using PatrimonioTech.Gui.DependencyInjection;
using PropertyChanged.SourceGenerator;
using ReactiveUI;

namespace PatrimonioTech.Gui.Login;

public partial class LoginViewModel : RoutableViewModelBase
{
    [Notify] private string _usuario = string.Empty;
    [Notify] private string _senha = string.Empty;

    public ReactiveCommand<Unit, Unit> Enter { get; }

    public IObservable<ImmutableList<string>> UsuariosExistentes { get; }

    public LoginViewModel(
        IScreen hostScreen,
        ICredentialGetUsersUseCase credentialGetUsersUseCase,
        ICredentialGetUserInfoUseCase credentialGetUserInfoUseCase,
        IFactory<DashboardViewModel> dashboard)
        : base(hostScreen)
    {
        var allUserCredentials = Observable
            .FromAsync(credentialGetUsersUseCase.Execute)
            .Select(x => x.UserNames);

        var whenUserChanged = this.WhenAnyValue(vm => vm.Usuario);
        var whenPasswordChanged = this.WhenAnyValue(vm => vm.Senha);

        var isUserSelected = whenUserChanged
            .CombineLatest(allUserCredentials)
            .Select(x => x.Second.Contains(x.First));
        var isPasswordValid = whenPasswordChanged
            .Select(x => x.Length >= Password.PasswordMinLength);

        var canLogin = Observable
            .CombineLatest(isUserSelected, isPasswordValid, (v1, v2) => v1 && v2)
            .DistinctUntilChanged();

        Enter = ReactiveCommand.Create(
            () => { },
            canLogin);

        UsuariosExistentes = allUserCredentials;

        var userInfos = Observable
            .CombineLatest(whenUserChanged, whenPasswordChanged, Enter, (user, password, _) => (user, password))
            .Select(x => new CredentialGetUserInfoRequest(x.user, x.password))
            .SelectMany(credentialGetUserInfoUseCase.Execute);

        this.WhenActivated(
            disposable =>
            {
                userInfos
                    .SelectMany(x => x.ToEnumerableSuccess())
                    .SwitchSelect(_ => hostScreen.Router.Navigate.Execute(dashboard.Create()))
                    .Subscribe(disposable);
            });
    }
}
