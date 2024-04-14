using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.GetUserAvailability;

[GenerateAutomaticInterface]
public sealed class UserGetAvailabilityUseCase(
    IUserCredentialRepository repository)
    : IUserGetAvailabilityUseCase
{
    public async Task<UserGetAvailabilityResponse> Execute(
        UserGetAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var userCredentials = await repository.GetAll(cancellationToken);

        return new UserGetAvailabilityResponse(
            userCredentials.Any(x => x.Name.Equals(request.UserName, StringComparison.CurrentCultureIgnoreCase)));
    }
}
