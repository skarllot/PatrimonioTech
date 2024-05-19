﻿namespace PatrimonioTech.Domain.Credentials.Services;

public interface IKeyDerivation
{
    CreateKeyResult CreateKey(Password password, int keySize, int iterations);

    Result<string, GetKeyError> TryGetKey(string password, string salt, string encryptedKey, int keySize, int iterations);
}

public sealed record CreateKeyResult(string Salt, string EncryptedKey);

public enum GetKeyError
{
    InvalidSalt = 1,
    InvalidEncryptedKey,
    InvalidPassword
}
