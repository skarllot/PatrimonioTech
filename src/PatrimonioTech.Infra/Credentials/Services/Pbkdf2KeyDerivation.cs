using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public sealed partial class Pbkdf2KeyDerivation : IKeyDerivation
{
    private const int BitsPerByte = 8;
    private const int AesMaxKeySize = 256;
    private const int AesMaxIvSize = 128;
    private static readonly HashAlgorithmName s_hashAlgorithmName = HashAlgorithmName.SHA512;

    private readonly ILogger<Pbkdf2KeyDerivation> _logger;

    public Pbkdf2KeyDerivation(ILogger<Pbkdf2KeyDerivation> logger)
    {
        _logger = logger;
    }

    public CreateKeyResult CreateKey(Password password, int keySize, int iterations)
    {
        Span<byte> binarySalt = stackalloc byte[keySize / BitsPerByte];
        RandomNumberGenerator.Fill(binarySalt);

        Span<byte> binaryKey = stackalloc byte[keySize / BitsPerByte];
        RandomNumberGenerator.Fill(binaryKey);

        var binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        Rfc2898DeriveBytes.Pbkdf2(password.Value, binarySalt, binaryHash, iterations, s_hashAlgorithmName);

        Span<byte> encrypted = stackalloc byte[keySize];

        using var aes = Aes.Create();
        aes.Key = binaryHash;
        var writtenBytes = aes.EncryptCbc(binaryKey, binarySalt[..(AesMaxIvSize / BitsPerByte)], encrypted);

        return new CreateKeyResult(
            Salt: Convert.ToBase64String(binarySalt),
            EncryptedKey: Convert.ToBase64String(encrypted[..writtenBytes]));
    }

    public Result<string, GetKeyError> TryGetKey(
        string password,
        string salt,
        string encryptedKey,
        int keySize,
        int iterations)
    {
        Span<byte> binarySalt = stackalloc byte[keySize / BitsPerByte];
        if (!Convert.TryFromBase64Chars(salt, binarySalt, out var saltBytes) || saltBytes != binarySalt.Length)
        {
            return GetKeyError.InvalidSalt;
        }

        Span<byte> binaryEncrypted = stackalloc byte[keySize];
        if (!Convert.TryFromBase64Chars(encryptedKey, binaryEncrypted, out var encryptedBytes))
        {
            return GetKeyError.InvalidEncryptedKey;
        }

        var binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        Rfc2898DeriveBytes.Pbkdf2(password, binarySalt, binaryHash, iterations, s_hashAlgorithmName);

        Span<byte> binaryKey = stackalloc byte[keySize / BitsPerByte];

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

    [LoggerMessage(LogLevel.Error, "Error decrypting CBC")]
    private partial void LogCryptographicException(CryptographicException exception);
}
