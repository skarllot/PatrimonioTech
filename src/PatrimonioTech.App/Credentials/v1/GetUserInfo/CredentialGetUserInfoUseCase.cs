using FxKit.Extensions;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.GetUserInfo;

[GenerateAutomaticInterface]
public sealed class CredentialGetUserInfoUseCase(
    IUserCredentialRepository repository,
    IKeyDerivation keyDerivation)
    : ICredentialGetUserInfoUseCase
{
    public Task<Result<CredentialGetUserInfoResponse, CredentialGetUserInfoError>> Execute(
        CredentialGetUserInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        return from userCredentials in repository.GetAll(cancellationToken)
                !.ToOptionAsync()
                .OkOrElseT(CredentialGetUserInfoError.Unexpected.Of)
            from foundUser in userCredentials
                .FirstOrNone(x => x.Name.Equals(request.UserName, StringComparison.CurrentCultureIgnoreCase))
                .OkOrElse(CredentialGetUserInfoError.UserNotFound.Of)
                .ToTask()
            from password in keyDerivation
                .TryGetKey(request.Password, foundUser.PasswordHash)
                .MapErr(CredentialGetUserInfoError.CryptographyError.Of)
                .ToTask()
            select new CredentialGetUserInfoResponse(
                Name: foundUser.Name,
                Database: foundUser.Database,
                DatabasePassword: password);
    }
}
