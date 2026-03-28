namespace PatrimonioTech.Infra.Credentials;

public sealed record UserCredentialModel(string Name, string PasswordHash, Guid Database);
