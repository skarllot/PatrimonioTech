using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.App.Credentials.v1.GetUserAvailability;
using PatrimonioTech.Gui.Common;
using PropertyChanged.SourceGenerator;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace PatrimonioTech.Gui.Users.Create;

public partial class UserCreateViewModel : RoutableViewModelBase
{
    // Source Streams
    [Notify] private string _userName = string.Empty;
    [Notify] private string _password = string.Empty;
    [Notify] private string _passwordConfirmation = string.Empty;

    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public ReactiveCommand<Unit, Result<Unit, CredentialAddUserResult>> Create { get; }

    public UserCreateViewModel(
        IScreen hostScreen,
        IUserGetAvailabilityUseCase getUserAvailabilityUseCase,
        ICredentialAddUserUseCase addUserUseCase)
        : base(hostScreen)
    {
        // USE CASES
        var getUserAvailability = (string userName) => Observable
            .Return(new UserGetAvailabilityRequest(userName))
            .SelectMany(getUserAvailabilityUseCase.Execute);

        var addUser = ((string UserName, string Password) r) => Observable
            .Return(new CredentialAddUserRequest(r.UserName, r.Password))
            .SelectMany(addUserUseCase.Execute);

        // USERNAME INPUT
        var whenUserNameChanged = this.WhenAnyValue(vm => vm.UserName);

        var isUserValid = whenUserNameChanged
            .Select(v => !v.AsSpan().Trim().IsWhiteSpace());

        var isUserAvailable = whenUserNameChanged
            .SelectMany(getUserAvailability)
            .Select(r => !r.Exists);

        // PASSWORD INPUT
        var whenPasswordChanged = this.WhenAnyValue(vm => vm.Password);

        var isPasswordValid = whenPasswordChanged
            .Select(x => x.Length >= Domain.Credentials.Password.PasswordMinLength);

        // PASSWORD CONFIRMATION INPUT
        var isConfirmationMatch = Observable
            .CombineLatest(whenPasswordChanged, this.WhenAnyValue(vm => vm.PasswordConfirmation), (v1, v2) => v1 == v2);

        // CANCEL COMMAND
        Cancel = ReactiveCommand.Create(() => { hostScreen.Router.NavigateBack.Execute(); });

        // CREATE COMMAND
        var canCreate = Observable
            .CombineLatest(
                isUserValid,
                isPasswordValid,
                isConfirmationMatch,
                isUserAvailable,
                (v1, v2, v3, v4) => v1 && v2 && v3 && v4)
            .DistinctUntilChanged();

        Create = ReactiveCommand.CreateFromObservable(
            (Unit _) => addUser((UserName, Password)),
            canCreate);

        // VALIDATIONS
        this.WhenActivated(
            disposable =>
            {
                this.ValidationRule(vm => vm.UserName, isUserAvailable, "Usuário já existe").DisposeWith(disposable);
                this.ValidationRule(vm => vm.Password, isPasswordValid, "Senha muito curta").DisposeWith(disposable);

                this
                    .ValidationRule(vm => vm.PasswordConfirmation, isConfirmationMatch, "As senhas não coincidem")
                    .DisposeWith(disposable);

                Create
                    .SelectMany(_ => hostScreen.Router.NavigateBack.Execute())
                    .Subscribe(disposable);
            });
    }
}
