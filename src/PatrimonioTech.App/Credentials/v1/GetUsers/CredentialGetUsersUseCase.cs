using System.Collections.Immutable;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.GetUsers;

[GenerateAutomaticInterface]
public class CredentialGetUsersUseCase(
    IUserCredentialRepository userCredentialRepository)
    : ICredentialGetUsersUseCase
{
    public async Task<CredentialGetUsersResponse> Execute(CancellationToken cancellationToken)
    {
        var credentials = await userCredentialRepository.GetAll(cancellationToken).ConfigureAwait(false);

        return new CredentialGetUsersResponse(GetUserNameQueryStrategy.Run(credentials).ToImmutableList());
    }
}
