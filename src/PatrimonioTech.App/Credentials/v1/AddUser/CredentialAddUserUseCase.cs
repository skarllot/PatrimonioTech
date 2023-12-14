using LanguageExt;
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
    public EitherAsync<CredentialAddUserResult, Unit> Execute(
        CredentialAddUserRequest request,
        CancellationToken cancellationToken)
    {
        return from scnRes in addUserScenario
                .Execute(new AddUserCredential(request.Name, request.Password, request.KeySize, request.Iterations))
                .MapLeft<CredentialAddUserResult>(e => e)
            let model = UserCredential.Create(scnRes)
            from repoRes in userCredentialRepository.Add(model, cancellationToken)
                .MapLeft<CredentialAddUserResult>(e => e)
            select Unit.Default;
    }
}

[GenerateOneOf]
public partial class CredentialAddUserResult : OneOfBase<
    AddUserCredentialError,
    UserCredentialAddError>;
