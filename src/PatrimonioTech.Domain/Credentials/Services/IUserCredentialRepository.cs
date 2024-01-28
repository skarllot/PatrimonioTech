using PatrimonioTech.Domain.Common.ValueObjects;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IUserCredentialRepository
{
    Task<Result<Unit, UserCredentialAddError>> Add(UserCredential userCredential, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken);
}

public enum UserCredentialAddError
{
    NameAlreadyExists = 1
}
