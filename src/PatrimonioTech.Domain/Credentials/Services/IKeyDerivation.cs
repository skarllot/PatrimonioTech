using Dunet;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IKeyDerivation
{
    CreateKeyResult CreateKey(Password password, int keySize, int iterations);
    Either<GetKeyError, string> TryGetKey(string password, string salt, string encryptedKey, int keySize, int iterations);
}

public sealed record CreateKeyResult(string Salt, string EncryptedKey);

[Union]
public partial record GetKeyError
{
    partial record InvalidSalt(string Salt);

    partial record InvalidEncryptedKey(string EncryptedKey);
}
