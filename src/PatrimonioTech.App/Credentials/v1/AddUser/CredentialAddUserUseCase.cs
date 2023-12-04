using LanguageExt;
using LanguageExt.Common;
using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

[GenerateAutomaticInterface]
public class CredentialAddUserUseCase(
        IUserCredentialRepository userCredentialRepository,
        IAddUserScenario addUserScenario)
    : ICredentialAddUserUseCase
{
    public async Task<Result<Unit>> Execute(CredentialAddUserRequest request, CancellationToken cancellationToken)
    {
        var _ = await addUserScenario.Execute(
                new AddUserCredential(request.Name, request.Password, request.KeySize, request.Iterations))
            .Map(e => UserCredential.Create(e))
            .MapAsync(u => userCredentialRepository.Add(u, cancellationToken));

        return default;
    }
}
