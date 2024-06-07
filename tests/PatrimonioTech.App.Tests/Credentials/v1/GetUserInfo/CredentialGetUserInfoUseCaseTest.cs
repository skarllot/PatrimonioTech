using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.App.Credentials.v1.GetUserInfo;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Tests.Credentials.v1.GetUserInfo;

[TestSubject(typeof(CredentialGetUserInfoUseCase))]
public class CredentialGetUserInfoUseCaseTest
{
    private readonly IUserCredentialRepository _userCredentialRepository = Substitute.For<IUserCredentialRepository>();
    private readonly IKeyDerivation _keyDerivation = Substitute.For<IKeyDerivation>();
    private readonly CredentialGetUserInfoUseCase _sut;

    public CredentialGetUserInfoUseCaseTest()
    {
        _userCredentialRepository.GetAll(CancellationToken.None).Returns(CredentialsFixture);

        _sut = new CredentialGetUserInfoUseCase(
            _userCredentialRepository,
            _keyDerivation);
    }

    [Fact]
    public async Task GivenARequest_WhenUserExistsAndPasswordIsValid_ThenReturnsUserInfo()
    {
        // Arrange
        _keyDerivation
            .TryGetKey("password", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns("secret");

        // Act
        var result = await _sut.Execute(new CredentialGetUserInfoRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeOk().DatabasePassword.Should().Be("secret");
    }

    [Fact]
    public async Task GivenARequest_WhenUserDoesNotExists_ThenReturnsError()
    {
        // Act
        var result = await _sut.Execute(new CredentialGetUserInfoRequest("laura", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialGetUserInfoError.UserNotFound>();
    }

    [Fact]
    public async Task GivenARequest_WhenUserExistsAndPasswordIsInvalid_ThenReturnsError()
    {
        // Arrange
        _keyDerivation
            .TryGetKey("password", Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(GetKeyError.InvalidPassword);

        // Act
        var result = await _sut.Execute(new CredentialGetUserInfoRequest("john", "password"), CancellationToken.None);

        // Assert
        result.Should().BeErr().Should().BeOfType<CredentialGetUserInfoError.CryptographyError>();
    }

    private static IReadOnlyList<UserCredential> CredentialsFixture =>
    [
        UserCredential.Create(new UserCredentialAdded("John", "123", "password", Guid.Empty, 1, 1)),
        UserCredential.Create(new UserCredentialAdded("Carl", "456", "password", Guid.Empty, 2, 1)),
        UserCredential.Create(new UserCredentialAdded("Anne", "789", "password", Guid.Empty, 3, 1)),
    ];
}
