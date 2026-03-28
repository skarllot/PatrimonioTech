using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public interface IPbkdf2PhcStringParser
{
    Pbkdf2PhcString Create(string salt, string encryptedKey, int iterations, int keyLengthBits);

    Result<Pbkdf2PhcString, GetKeyError> Parse(ReadOnlySpan<char> value);
}
