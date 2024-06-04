namespace PatrimonioTech.Domain.Credentials.Services;

public enum GetKeyError
{
    InvalidSalt = 1,
    InvalidEncryptedKey,
    InvalidPassword,
}
