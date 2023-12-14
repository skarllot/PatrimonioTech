using OneOf;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IUserCredentialRepository
{
    EitherAsync<UserCredentialAddError, Unit> Add(UserCredential userCredential, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken);
}

[GenerateOneOf]
public partial class UserCredentialAddError : OneOfBase<UserCredentialAddError.NameAlreadyExists>
{
    public sealed record NameAlreadyExists(string Name);
}
