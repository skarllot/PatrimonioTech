using CSharpFunctionalExtensions;
using OneOf;
using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.GetUserInfo;

[GenerateAutomaticInterface]
public class CredentialGetUserInfoUseCase(
    IUserCredentialRepository repository,
    IKeyDerivation keyDerivation)
    : ICredentialGetUserInfoUseCase
{
    public Task<Result<CredentialGetUserInfoResponse, CredentialGetUserInfoError>> Execute(
        CredentialGetUserInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        return from userCredentials in repository.GetAll(cancellationToken)
                !.ToResultAsync(CredentialGetUserInfoError.Other.Unexpected)
                .MapError(e => (CredentialGetUserInfoError)e)
            from foundUser in userCredentials
                .TryFirst(x => x.Name.Equals(request.UserName, StringComparison.CurrentCultureIgnoreCase))
                .ToResult(CredentialGetUserInfoError.Other.UserNotFound)
                .MapError(e => (CredentialGetUserInfoError)e)
            from password in keyDerivation
                .TryGetKey(request.Password, foundUser.Salt, foundUser.Key, foundUser.KeySize, foundUser.Iterations)
                .MapError(e => (CredentialGetUserInfoError)e)
            select new CredentialGetUserInfoResponse(foundUser.Name, foundUser.Database, password);
    }
}

[GenerateOneOf]
public partial class CredentialGetUserInfoError : OneOfBase<
    GetKeyError,
    CredentialGetUserInfoError.Other>
{
    public enum Other
    {
        Unexpected,
        UserNotFound
    }
}
