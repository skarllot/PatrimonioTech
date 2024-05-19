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
                .TryGetKey(
                    password: request.Password,
                    salt: foundUser.Salt,
                    encryptedKey: foundUser.Key,
                    keySize: foundUser.KeySize,
                    iterations: foundUser.Iterations)
                .MapError(e => (CredentialGetUserInfoError)e)
            select new CredentialGetUserInfoResponse(
                Name: foundUser.Name,
                Database: foundUser.Database,
                DatabasePassword: password);
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
