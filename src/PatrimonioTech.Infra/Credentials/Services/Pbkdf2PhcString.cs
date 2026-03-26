using System.Globalization;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

/// <summary>
/// A PHC-format string encoding PBKDF2-SHA512 parameters and an AES-CBC encrypted database key.
/// Format: $pbkdf2-sha512$i={iterations},l={keyLengthBits}${saltBase64}${encryptedKeyBase64}
/// </summary>
internal sealed class Pbkdf2PhcString : IPhcString
{
    private const string AlgorithmId = "pbkdf2-sha512";

    private Pbkdf2PhcString(string value, string salt, string encryptedKey, int iterations, int keyLengthBits)
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

    internal static Pbkdf2PhcString Create(string salt, string encryptedKey, int iterations, int keyLengthBits)
    {
        var value = $"${AlgorithmId}$i={iterations},l={keyLengthBits}${salt}${encryptedKey}";
        return new Pbkdf2PhcString(value, salt, encryptedKey, iterations, keyLengthBits);
    }

    internal static Result<Pbkdf2PhcString, GetKeyError> Parse(string value)
    {
        var parts = value.Split('$');

        // Expected: ["", "pbkdf2-sha512", "i=100000,l=512", saltBase64, encryptedKeyBase64]
        if (parts.Length != 5 || !string.Equals(parts[0], string.Empty, StringComparison.Ordinal) || !string.Equals(parts[1], AlgorithmId, StringComparison.Ordinal))
            return GetKeyError.InvalidPassword;

        if (!TryParseParams(parts[2], out var iterations, out var keyLengthBits))
            return GetKeyError.InvalidPassword;

        return new Pbkdf2PhcString(value, parts[3], parts[4], iterations, keyLengthBits);
    }

    private static bool TryParseParams(string paramString, out int iterations, out int keyLengthBits)
    {
        iterations = 0;
        keyLengthBits = 0;

        foreach (var param in paramString.Split(','))
        {
            var eqIdx = param.IndexOf('=');
            if (eqIdx < 0)
                return false;

            var key = param[..eqIdx];
            if (!int.TryParse(param[(eqIdx + 1)..], CultureInfo.InvariantCulture, out var value))
                return false;

            if (string.Equals(key, "i", StringComparison.Ordinal))
                iterations = value;
            else if (string.Equals(key, "l", StringComparison.Ordinal))
                keyLengthBits = value;
        }

        return iterations > 0 && keyLengthBits > 0;
    }
}
