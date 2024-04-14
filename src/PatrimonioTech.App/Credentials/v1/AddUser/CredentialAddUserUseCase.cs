using System.Reactive;
using CSharpFunctionalExtensions;
using OneOf;
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
    public Task<Result<Unit, CredentialAddUserResult>> Execute(
        CredentialAddUserRequest request,
        CancellationToken cancellationToken)
    {
        return from scnRes in addUserScenario
                .Execute(new AddUserCredential(request.Name, request.Password, request.KeySize, request.Iterations))
                .MapError(e => (CredentialAddUserResult)e)
            let model = UserCredential.Create(scnRes)
            from repoRes in userCredentialRepository.Add(model, cancellationToken)
                .MapError(e => (CredentialAddUserResult)e)
            select Unit.Default;
    }
}

[GenerateOneOf]
public partial class CredentialAddUserResult : OneOfBase<
    AddUserCredentialError,
    UserCredentialAddError>;
