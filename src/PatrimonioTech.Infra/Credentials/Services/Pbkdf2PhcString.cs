using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

/// <summary>
/// A PHC-format string encoding PBKDF2-SHA512 parameters and an AES-256-CBC encrypted database key.
/// Format: $pbkdf2-sha512-aes256cbc$i={iterations},l={keyLengthBits}${saltBase64}${encryptedKeyBase64}
/// </summary>
public sealed class Pbkdf2PhcString : IPhcString
{
    internal Pbkdf2PhcString(string value, string salt, string encryptedKey, int iterations, int keyLengthBits)
    {
        Value = value;
        Salt = salt;
        EncryptedKey = encryptedKey;
        Iterations = iterations;
        KeyLengthBits = keyLengthBits;
    }

    public string Value { get; }
    internal string Salt { get; }
    internal string EncryptedKey { get; }
    internal int Iterations { get; }
    internal int KeyLengthBits { get; }
}
