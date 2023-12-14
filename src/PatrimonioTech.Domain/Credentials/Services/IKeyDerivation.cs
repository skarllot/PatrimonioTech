using OneOf;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IKeyDerivation
{
    CreateKeyResult CreateKey(Password password, int keySize, int iterations);
    Either<GetKeyError, string> TryGetKey(string password, string salt, string encryptedKey, int keySize, int iterations);
}

public sealed record CreateKeyResult(string Salt, string EncryptedKey);

[GenerateOneOf]
public partial class GetKeyError : OneOfBase<GetKeyError.InvalidSalt, GetKeyError.InvalidEncryptedKey>
{
    public sealed record InvalidSalt(string Salt);

    public sealed record InvalidEncryptedKey(string EncryptedKey);
}
