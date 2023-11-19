using LanguageExt.Common;

namespace PatrimonioTech.Domain.Credentials.Services;

public interface IKeyDerivation
{
    (string Salt, string EncryptedKey) CreateKey(string password, int keySize, int iterations);
    Result<string> TryGetKey(string password, string salt, string encryptedKey, int keySize, int iterations);
}
