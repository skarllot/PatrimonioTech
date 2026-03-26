using FluentAssertions;
using JetBrains.Annotations;
using NSubstitute;
using PatrimonioTech.App.Credentials.v1.GetUsers;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Tests.Credentials.v1.GetUsers;

[TestSubject(typeof(CredentialGetUsersUseCase))]
public class CredentialGetUsersUseCaseTest
{
    private readonly IUserCredentialRepository _userCredentialRepository = Substitute.For<IUserCredentialRepository>();
    private readonly CredentialGetUsersUseCase _sut;

    public CredentialGetUsersUseCaseTest()
    {
        _sut = new CredentialGetUsersUseCase(_userCredentialRepository);
    }

    [Fact]
    public async Task GivenARequest_WhenCredentialsExists_ThenReturnsList()
    {
        // Arrange
        _userCredentialRepository
            .GetAll(CancellationToken.None)
            .Returns(
            [
                UserCredential.Create(new UserCredentialAdded("John", "$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt1$key1", Guid.Empty)),
                UserCredential.Create(new UserCredentialAdded("Carl", "$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt2$key2", Guid.Empty)),
                UserCredential.Create(new UserCredentialAdded("Anne", "$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt3$key3", Guid.Empty)),
            ]);

        // Act
        var response = await _sut.Execute(CancellationToken.None);

        // Assert
        response.UserNames.Should().HaveCount(3);
    }

    [Fact]
    public async Task GivenARequest_WhenNoCredentialsExists_ThenReturnsEmptyList()
    {
        // Arrange
        _userCredentialRepository
            .GetAll(CancellationToken.None)
            .Returns([]);

        // Act
        var response = await _sut.Execute(CancellationToken.None);

        // Assert
        response.UserNames.Should().BeEmpty();
    }
}
