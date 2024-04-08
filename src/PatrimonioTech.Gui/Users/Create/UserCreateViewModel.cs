using System.Reactive;
using CSharpFunctionalExtensions;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PropertyChanged.SourceGenerator;
using ReactiveUI;

namespace PatrimonioTech.Gui.Users.Create;

public partial class UserCreateViewModel : RoutableViewModelBase
{
    [Notify] private string _userName = string.Empty;
    [Notify] private string _password = string.Empty;
    [Notify] private string _passwordConfirmation = string.Empty;

    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public ReactiveCommand<Unit, Result<Domain.Common.ValueObjects.Unit, CredentialAddUserResult>> Create { get; }

    public UserCreateViewModel(IScreen hostScreen, ICredentialAddUserUseCase addUserUseCase) : base(hostScreen)
    {
        Cancel = ReactiveCommand.Create(() => { hostScreen.Router.NavigateBack.Execute(); });
        Create = ReactiveCommand.CreateFromTask(ct => addUserUseCase.Execute(new CredentialAddUserRequest(UserName, Password), ct));
    }
}
