using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.App.Credentials.v1.GetUserAvailability;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Tests.Credentials.v1.GetUserAvailability;

[TestSubject(typeof(UserGetAvailabilityUseCase))]
public class UserGetAvailabilityUseCaseTest
{
    private readonly IUserCredentialRepository _userCredentialRepository = Substitute.For<IUserCredentialRepository>();
    private readonly UserGetAvailabilityUseCase _sut;

    public UserGetAvailabilityUseCaseTest()
    {
        _userCredentialRepository.GetAll(CancellationToken.None).Returns(CredentialsFixture);
        _sut = new UserGetAvailabilityUseCase(_userCredentialRepository);
    }

    [Fact]
    public async Task GivenARequest_WhenMatchNotFound_ThenReturnsFalse()
    {
        // Act
        var response = await _sut.Execute(new UserGetAvailabilityRequest("test"), CancellationToken.None);

        // Assert
        response.Exists.Should().BeFalse();
    }

    [Fact]
    public async Task GivenARequest_WhenExactMatchIsFound_ThenReturnsTrue()
    {
        // Act
        var response = await _sut.Execute(new UserGetAvailabilityRequest("Anne"), CancellationToken.None);

        // Assert
        response.Exists.Should().BeTrue();
    }

    [Fact]
    public async Task GivenARequest_WhenDifferentCaseMatchIsFound_ThenReturnsTrue()
    {
        // Act
        var response = await _sut.Execute(new UserGetAvailabilityRequest("anne"), CancellationToken.None);

        // Assert
        response.Exists.Should().BeTrue();
    }

    private static IReadOnlyList<UserCredential> CredentialsFixture =>
    [
        UserCredential.Create(new UserCredentialAdded("John", "$pbkdf2-sha512$i=100000,l=512$salt1$key1", Guid.Empty)),
        UserCredential.Create(new UserCredentialAdded("Carl", "$pbkdf2-sha512$i=100000,l=512$salt2$key2", Guid.Empty)),
        UserCredential.Create(new UserCredentialAdded("Anne", "$pbkdf2-sha512$i=100000,l=512$salt3$key3", Guid.Empty)),
    ];
}
