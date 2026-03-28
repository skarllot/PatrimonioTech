using System.Globalization;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public sealed class Pbkdf2PhcStringParser : IPbkdf2PhcStringParser
{
    private const string AlgorithmId = "pbkdf2-sha512-aes256cbc";

    public Pbkdf2PhcString Create(string salt, string encryptedKey, int iterations, int keyLengthBits)
    {
        var value = $"${AlgorithmId}$i={iterations},l={keyLengthBits}${salt}${encryptedKey}";
        return new Pbkdf2PhcString(value, salt, encryptedKey, iterations, keyLengthBits);
    }

    public Result<Pbkdf2PhcString, GetKeyError> Parse(ReadOnlySpan<char> value)
    {
        Span<Range> ranges = stackalloc Range[6];
        var count = value.Split(ranges, '$');

        if (count != 5 || !value[ranges[0]].IsEmpty || !value[ranges[1]].Equals(AlgorithmId, StringComparison.Ordinal))
            return GetKeyError.InvalidHash;

        if (!TryParseParams(value[ranges[2]], out var iterations, out var keyLengthBits))
            return GetKeyError.InvalidHash;

        return new Pbkdf2PhcString(value.ToString(), value[ranges[3]].ToString(), value[ranges[4]].ToString(), iterations, keyLengthBits);
    }

    private static bool TryParseParams(ReadOnlySpan<char> paramString, out int iterations, out int keyLengthBits)
    {
        iterations = 0;
        keyLengthBits = 0;

        foreach (var chunk in paramString.Split(','))
        {
            var param = paramString[chunk];
            var eqIdx = param.IndexOf('=');
            if (eqIdx < 0)
                return false;

            var key = param[..eqIdx];
            if (!int.TryParse(param[(eqIdx + 1)..], CultureInfo.InvariantCulture, out var val))
                return false;

            if (key.Equals("i", StringComparison.Ordinal))
                iterations = val;
            else if (key.Equals("l", StringComparison.Ordinal))
                keyLengthBits = val;
            else
                return false;
        }

        return iterations > 0 && keyLengthBits > 0;
    }
}
