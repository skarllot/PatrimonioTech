using LanguageExt;
using LanguageExt.Common;
using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

[GenerateAutomaticInterface]
public class CredentialAddUserUseCase(
        IUserCredentialRepository userCredentialRepository,
        IKeyDerivation keyDerivation)
    : ICredentialAddUserUseCase
{
    public async Task<Result<Unit>> Execute(CredentialAddUserRequest request, CancellationToken cancellationToken)
    {
        (string? salt, string? encryptedKey) = keyDerivation
            .CreateKey(request.Password, request.KeySize, request.Iterations);

        var newUser = new UserCredential(
            request.Name,
            salt,
            encryptedKey,
            Guid.NewGuid(),
            request.KeySize,
            request.Iterations);

        return await userCredentialRepository
            .Add(newUser, cancellationToken)
            .ConfigureAwait(false);
    }
}
