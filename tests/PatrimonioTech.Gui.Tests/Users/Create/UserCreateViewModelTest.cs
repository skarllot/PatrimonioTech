using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.App.Credentials.v1.GetUserAvailability;
using PatrimonioTech.App.Database;
using PatrimonioTech.Gui.Users.Create;
using ReactiveUI;
using ReactiveUI.Builder;

namespace PatrimonioTech.Gui.Tests.Users.Create;

[TestSubject(typeof(UserCreateViewModel))]
public class UserCreateViewModelTest : IDisposable
{
    private readonly IScheduler _previousScheduler;
    private readonly IUserGetAvailabilityUseCase _availabilityUseCase;
    private readonly ICredentialAddUserUseCase _addUserUseCase;
    private readonly RoutingState _routingState;
    private readonly IScreen _screen;

    static UserCreateViewModelTest()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }

    public UserCreateViewModelTest()
    {
        _previousScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;

        _availabilityUseCase = Substitute.For<IUserGetAvailabilityUseCase>();
        _addUserUseCase = Substitute.For<ICredentialAddUserUseCase>();
        _routingState = new RoutingState();
        _screen = Substitute.For<IScreen>();
        _screen.Router.Returns(_routingState);

        _availabilityUseCase
            .Execute(Arg.Any<UserGetAvailabilityRequest>(), Arg.Any<CancellationToken>())
            .Returns(new UserGetAvailabilityResponse(false));
    }

    public void Dispose() => RxSchedulers.MainThreadScheduler = _previousScheduler;

    private (UserCreateViewModel Vm, IDisposable Activation) CreateActivated()
    {
        var vm = new UserCreateViewModel(_screen, _availabilityUseCase, _addUserUseCase);
        var activation = vm.Activator.Activate();
        return (vm, activation);
    }

    [Fact]
    public void GivenEmptyUsername_WhenActivated_CreateIsDisabled()
    {
        var (vm, _) = CreateActivated();

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);

        canCreate.Should().BeFalse();
    }

    [Fact]
    public void GivenUsernameShorterThan3Chars_WhenActivated_ShowsNomeMuitoCurtoAndCreateIsDisabled()
    {
        var (vm, _) = CreateActivated();
        vm.UserName = "ab";
        vm.Password = "password123";
        vm.PasswordConfirmation = "password123";

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);
        var errors = ((INotifyDataErrorInfo)vm).GetErrors(nameof(vm.UserName)).Cast<string>().ToList();

        canCreate.Should().BeFalse();
        errors.Should().Contain("Nome muito curto");
    }

    [Fact]
    public void GivenPasswordShorterThan8Chars_WhenActivated_ShowsSenhaMuitoCurtaAndCreateIsDisabled()
    {
        var (vm, _) = CreateActivated();
        vm.UserName = "alice";
        vm.Password = "pass";
        vm.PasswordConfirmation = "pass";

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);
        var errors = ((INotifyDataErrorInfo)vm).GetErrors(nameof(vm.Password)).Cast<string>().ToList();

        canCreate.Should().BeFalse();
        errors.Should().Contain("Senha muito curta");
    }

    [Fact]
    public void GivenMismatchedPasswordConfirmation_WhenActivated_ShowsAsSenhasNaoCoincidemAndCreateIsDisabled()
    {
        var (vm, _) = CreateActivated();
        vm.UserName = "alice";
        vm.Password = "password123";
        vm.PasswordConfirmation = "different123";

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);
        var errors = ((INotifyDataErrorInfo)vm).GetErrors(nameof(vm.PasswordConfirmation)).Cast<string>().ToList();

        canCreate.Should().BeFalse();
        errors.Should().Contain("As senhas não coincidem");
    }

    [Fact]
    public void GivenExistingUsername_WhenActivated_ShowsUsuarioJaExisteAndCreateIsDisabled()
    {
        _availabilityUseCase
            .Execute(Arg.Any<UserGetAvailabilityRequest>(), Arg.Any<CancellationToken>())
            .Returns(new UserGetAvailabilityResponse(true));

        var (vm, _) = CreateActivated();
        vm.UserName = "existinguser";
        vm.Password = "password123";
        vm.PasswordConfirmation = "password123";

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);
        var errors = ((INotifyDataErrorInfo)vm).GetErrors(nameof(vm.UserName)).Cast<string>().ToList();

        canCreate.Should().BeFalse();
        errors.Should().Contain("Usuário já existe");
    }

    [Fact]
    public void GivenAllFieldsValid_WhenUsernameIsAvailable_CreateIsEnabled()
    {
        var (vm, _) = CreateActivated();
        vm.UserName = "alice";
        vm.Password = "password123";
        vm.PasswordConfirmation = "password123";

        var canCreate = false;
        vm.Create.CanExecute.Subscribe(x => canCreate = x);

        canCreate.Should().BeTrue();
    }

    [Fact]
    public async Task GivenSuccessfulCreate_WhenExecuted_NavigatesBack()
    {
        _addUserUseCase
            .Execute(Arg.Any<CredentialAddUserRequest>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Default);

        var vm = new UserCreateViewModel(_screen, _availabilityUseCase, _addUserUseCase);
        _routingState.NavigationStack.Add(vm);
        using var activation = vm.Activator.Activate();

        vm.UserName = "alice";
        vm.Password = "password123";
        vm.PasswordConfirmation = "password123";

        await vm.Create.Execute();

        _routingState.NavigationStack.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenFailedCreate_WhenExecuted_ShowErrorInteractionTriggeredWithCorrectMessage()
    {
        _addUserUseCase
            .Execute(Arg.Any<CredentialAddUserRequest>(), Arg.Any<CancellationToken>())
            .Returns(new CredentialAddUserError.DatabaseError((CreateDatabaseError)0));

        var (vm, _) = CreateActivated();
        vm.UserName = "alice";
        vm.Password = "password123";
        vm.PasswordConfirmation = "password123";

        string? shownMessage = null;
        vm.ShowError.RegisterHandler(ctx =>
        {
            shownMessage = ctx.Input;
            ctx.SetOutput(Unit.Default);
        });

        await vm.Create.Execute();

        shownMessage.Should()
            .Be("Não foi possível criar o banco de dados. Verifique o espaço em disco e tente novamente.");
    }
}
