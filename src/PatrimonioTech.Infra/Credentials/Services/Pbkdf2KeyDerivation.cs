﻿using System.Security.Cryptography;
using LanguageExt.Common;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public class Pbkdf2KeyDerivation : IKeyDerivation
{
    private const int BitsPerByte = 8;
    private const int AesMaxKeySize = 256;
    private const int AesMaxIvSize = 128;
    private static readonly HashAlgorithmName s_hashAlgorithmName = HashAlgorithmName.SHA512;

    public (string Salt, string EncryptedKey) CreateKey(string password, int keySize, int iterations)
    {
        Span<byte> binarySalt = stackalloc byte[keySize / BitsPerByte];
        RandomNumberGenerator.Fill(binarySalt);

        Span<byte> binaryKey = stackalloc byte[keySize / BitsPerByte];
        RandomNumberGenerator.Fill(binaryKey);

        byte[] binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        Rfc2898DeriveBytes.Pbkdf2(password, binarySalt, binaryHash, iterations, s_hashAlgorithmName);

        Span<byte> encrypted = stackalloc byte[keySize];

        using Aes aes = Aes.Create();
        aes.Key = binaryHash;
        int writtenBytes = aes.EncryptCbc(binaryKey, binarySalt[..(AesMaxIvSize / BitsPerByte)], encrypted);

        return (Convert.ToBase64String(binarySalt), Convert.ToBase64String(encrypted[..writtenBytes]));
    }

    public Result<string> TryGetKey(
        string password,
        string salt,
        string encryptedKey,
        int keySize,
        int iterations)
    {
        Span<byte> binarySalt = stackalloc byte[keySize / BitsPerByte];
        if (!Convert.TryFromBase64Chars(salt, binarySalt, out int saltBytes) || saltBytes != binarySalt.Length)
        {
            return new Result<string>(new InvalidOperationException("Invalid salt"));
        }

        Span<byte> binaryEncrypted = stackalloc byte[keySize];
        if (!Convert.TryFromBase64Chars(encryptedKey, binaryEncrypted, out int encryptedBytes))
        {
            return new Result<string>(new InvalidOperationException("Invalid encrypted key"));
        }

        byte[] binaryHash = new byte[AesMaxKeySize / BitsPerByte];
        Rfc2898DeriveBytes.Pbkdf2(password, binarySalt, binaryHash, iterations, s_hashAlgorithmName);

        Span<byte> binaryKey = stackalloc byte[keySize / BitsPerByte];

        using Aes aes = Aes.Create();
        aes.Key = binaryHash;
        int keyBytes = aes.DecryptCbc(binaryEncrypted[..encryptedBytes], binarySalt[..(AesMaxIvSize / BitsPerByte)], binaryKey);

        return Convert.ToBase64String(binaryKey[..keyBytes]);
    }
}