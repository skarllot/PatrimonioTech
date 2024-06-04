namespace PatrimonioTech.Domain.Credentials.Services;

public sealed record CreateKeyResult(string Salt, string EncryptedKey);
