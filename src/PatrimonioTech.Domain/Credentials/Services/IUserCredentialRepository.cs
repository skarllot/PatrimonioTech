using Dunet;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IUserCredentialRepository
{
    Task<Either<UserCredentialAddError, Unit>> Add(UserCredential userCredential, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken);
}

[Union]
public partial record UserCredentialAddError
{
    partial record NameAlreadyExists(string Name);
}
