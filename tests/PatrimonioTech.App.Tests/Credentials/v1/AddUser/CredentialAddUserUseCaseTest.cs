using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.App.Database;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Tests.Credentials.v1.AddUser;

[TestSubject(typeof(CredentialAddUserUseCase))]
public class CredentialAddUserUseCaseTest
{
    private readonly IDatabaseAdmin _databaseAdmin = Substitute.For<IDatabaseAdmin>();
    private readonly IUserCredentialRepository _userCredentialRepository = Substitute.For<IUserCredentialRepository>();
    private readonly IAddUserScenario _addUserScenario = Substitute.For<IAddUserScenario>();
    private readonly IKeyDerivation _keyDerivation = Substitute.For<IKeyDerivation>();
    private readonly CredentialAddUserUseCase _sut;

    public CredentialAddUserUseCaseTest()
    {
        _sut = new CredentialAddUserUseCase(
            _databaseAdmin,
            _userCredentialRepository,
            _addUserScenario,
            _keyDerivation);
    }

    [Fact]
    public async Task GivenARequest_WhenItIsValid_ThenSaveUser()
    {
        // Arrange
        _addUserScenario
            .Execute(Arg.Any<AddUserCredential>())
            .Returns(new UserCredentialAdded("john", "123", "password", Guid.Empty, 1, 1));
        _userCredentialRepository
            .Add(Arg.Any<UserCredential>(), CancellationToken.None)
            .Returns(Unit.Default);
        _keyDerivation
            .TryGetKey(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns("password");
        _databaseAdmin
            .CreateDatabase(Guid.Empty, Arg.Any<string>())
            .Returns(Unit.Default);

        // Act
        var result = await _sut.Execute(new CredentialAddUserRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeOk();
    }

    [Fact]
    public async Task GivenARequest_WhenScenarioFails_ThenReturnError()
    {
        // Arrange
        _addUserScenario
            .Execute(Arg.Any<AddUserCredential>())
            .Returns(AddUserCredentialError.InvalidPassword.Of(PasswordError.TooShort));

        // Act
        var result = await _sut.Execute(new CredentialAddUserRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialAddUserError.BusinessError>();
        _keyDerivation.ReceivedCalls().Should().BeEmpty();
        _databaseAdmin.ReceivedCalls().Should().BeEmpty();
        _userCredentialRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task GivenARequest_WhenCryptographyFails_ThenReturnError()
    {
        // Arrange
        _addUserScenario
            .Execute(Arg.Any<AddUserCredential>())
            .Returns(new UserCredentialAdded("john", "123", "password", Guid.Empty, 1, 1));
        _userCredentialRepository
            .Add(Arg.Any<UserCredential>(), CancellationToken.None)
            .Returns(Unit.Default);
        _keyDerivation
            .TryGetKey(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(GetKeyError.InvalidPassword);

        // Act
        var result = await _sut.Execute(new CredentialAddUserRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialAddUserError.CryptographyError>();
        _databaseAdmin.ReceivedCalls().Should().BeEmpty();
        _userCredentialRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task GivenARequest_WhenDatabaseFails_ThenReturnError()
    {
        // Arrange
        _addUserScenario
            .Execute(Arg.Any<AddUserCredential>())
            .Returns(new UserCredentialAdded("john", "123", "password", Guid.Empty, 1, 1));
        _keyDerivation
            .TryGetKey(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns("password");
        _databaseAdmin
            .CreateDatabase(Guid.Empty, Arg.Any<string>())
            .Returns((CreateDatabaseError)0);

        // Act
        var result = await _sut.Execute(new CredentialAddUserRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialAddUserError.DatabaseError>();
        _userCredentialRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task GivenARequest_WhenRepositoryFails_ThenReturnError()
    {
        // Arrange
        _addUserScenario
            .Execute(Arg.Any<AddUserCredential>())
            .Returns(new UserCredentialAdded("john", "123", "password", Guid.Empty, 1, 1));
        _keyDerivation
            .TryGetKey(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns("password");
        _databaseAdmin
            .CreateDatabase(Guid.Empty, Arg.Any<string>())
            .Returns(Unit.Default);
        _userCredentialRepository
            .Add(Arg.Any<UserCredential>(), CancellationToken.None)
            .Returns(UserCredentialAddError.NameAlreadyExists);

        // Act
        var result = await _sut.Execute(new CredentialAddUserRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialAddUserError.StorageError>();
    }
}
