using LanguageExt;
using LanguageExt.Common;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IUserCredentialRepository
{
    Task<Result<Unit>> Add(UserCredential userCredential, CancellationToken cancellationToken);

    Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken);
}
