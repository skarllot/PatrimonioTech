using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public sealed class Pbkdf2PhcStringParser : IPbkdf2PhcStringParser
{
    public Pbkdf2PhcString Create(string salt, string encryptedKey, int iterations, int keyLengthBits) =>
        Pbkdf2PhcString.Create(salt, encryptedKey, iterations, keyLengthBits);

    public Result<Pbkdf2PhcString, GetKeyError> Parse(ReadOnlySpan<char> value) =>
        Pbkdf2PhcString.Parse(value);
}
