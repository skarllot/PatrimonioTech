using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public sealed partial class Pbkdf2KeyDerivation : IKeyDerivation
{
    private const int BitsPerByte = 8;
    private const int DefaultKeyLengthBits = 512;
    private const int DefaultIterations = 100_000;
    private const int AesMaxKeySize = 256;
    private const int AesMaxIvSize = 128;
    private static readonly HashAlgorithmName s_hashAlgorithmName = HashAlgorithmName.SHA512;

    private readonly ILogger<Pbkdf2KeyDerivation> _logger;

    public Pbkdf2KeyDerivation(ILogger<Pbkdf2KeyDerivation> logger)
    {
        _logger = logger;
    }

    public Result<IPhcString, CryptographyError> CreateKey(Password password)
    {
        var keyLengthBytes = DefaultKeyLengthBits / BitsPerByte;

        Span<byte> binarySalt = stackalloc byte[keyLengthBytes];
        RandomNumberGenerator.Fill(binarySalt);

        Span<byte> binaryKey = stackalloc byte[keyLengthBytes];
        RandomNumberGenerator.Fill(binaryKey);

        var binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        try
        {
            Rfc2898DeriveBytes.Pbkdf2(password.Value, binarySalt, binaryHash, DefaultIterations, s_hashAlgorithmName);
        }
        catch (CryptographicException e)
        {
            LogCryptographicException(e);
            return CryptographyError.KeyDerivationFailed;
        }

        Span<byte> encrypted = stackalloc byte[DefaultKeyLengthBits];
        int writtenBytes;
        try
        {
            using var aes = Aes.Create();
            aes.Key = binaryHash;
            writtenBytes = aes.EncryptCbc(binaryKey, binarySalt[..(AesMaxIvSize / BitsPerByte)], encrypted);
        }
        catch (CryptographicException e)
        {
            LogCryptographicException(e);
            return CryptographyError.EncryptionFailed;
        }

        return Pbkdf2PhcString.Create(
            salt: Convert.ToBase64String(binarySalt),
            encryptedKey: Convert.ToBase64String(encrypted[..writtenBytes]),
            iterations: DefaultIterations,
            keyLengthBits: DefaultKeyLengthBits);
    }

    public Result<string, GetKeyError> TryGetKey(string password, string passwordHash)
    {
        return from phcString in Pbkdf2PhcString.Parse(passwordHash)
               from key in DecryptKey(password, phcString)
               select key;
    }

    private Result<string, GetKeyError> DecryptKey(string password, Pbkdf2PhcString phcString)
    {
        var keyLengthBytes = phcString.KeyLengthBits / BitsPerByte;

        Span<byte> binarySalt = stackalloc byte[keyLengthBytes];
        if (!Convert.TryFromBase64Chars(phcString.Salt, binarySalt, out var saltBytes) || saltBytes != binarySalt.Length)
            return GetKeyError.InvalidPassword;

        Span<byte> binaryEncrypted = stackalloc byte[phcString.KeyLengthBits];
        if (!Convert.TryFromBase64Chars(phcString.EncryptedKey, binaryEncrypted, out var encryptedBytes))
            return GetKeyError.InvalidPassword;

        var binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        Rfc2898DeriveBytes.Pbkdf2(password, binarySalt, binaryHash, phcString.Iterations, s_hashAlgorithmName);

        Span<byte> binaryKey = stackalloc byte[keyLengthBytes];

        Aes? aes = null;
        try
        {
            aes = Aes.Create();
            aes.Key = binaryHash;

            if (aes.TryDecryptCbc(
                    binaryEncrypted[..encryptedBytes],
                    binarySalt[..(AesMaxIvSize / BitsPerByte)],
                    binaryKey,
                    out var bytesWritten))
            {
                return Convert.ToBase64String(binaryKey[..bytesWritten]);
            }
            else
            {
                return GetKeyError.InvalidPassword;
            }
        }
        catch (CryptographicException e)
        {
            LogCryptographicException(e);
            return GetKeyError.InvalidPassword;
        }
        finally
        {
            aes?.Dispose();
        }
    }

    [LoggerMessage(LogLevel.Error, "Error during cryptographic operation")]
    private partial void LogCryptographicException(CryptographicException exception);
}
