using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;
using PropertyChanged.SourceGenerator;
using ReactiveUI;

namespace PatrimonioTech.Gui.Login;

public partial class LoginViewModel : RoutableViewModelBase
{
    [Notify] private string _usuario = string.Empty;
    [Notify] private string _senha = string.Empty;

    public ReactiveCommand<Unit, Unit> Entrar { get; }

    public IObservable<List<string>> UsuariosExistentes { get; }

    public LoginViewModel(IScreen hostScreen, IUserCredentialRepository userCredentialRepository)
        : base(hostScreen)
    {
        var allUserCredentials = Observable
            .FromAsync(userCredentialRepository.GetAll)
            .Select(x => x.OrderBy(y => y.Name, StringComparer.CurrentCultureIgnoreCase));

        var isUserSelected = this.WhenAnyValue(vm => vm.Usuario)
            .CombineLatest(allUserCredentials)
            .Select(x => x.Second.Select(y => y.Name).Contains(x.First));
        var isPasswordValid = this.WhenAnyValue(vm => vm.Senha)
            .Select(x => x.Length >= Password.PasswordMinLength);

        var canLogin = Observable
            .CombineLatest(isUserSelected, isPasswordValid, (v1, v2) => v1 && v2)
            .DistinctUntilChanged();

        Entrar = ReactiveCommand.Create(
            () => { },
            canLogin);

        UsuariosExistentes = allUserCredentials
            .Select(x => x.Select(y => y.Name).ToList());

        this.WhenActivated((CompositeDisposable _) => { });
    }
}
