namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

public sealed record UserCredentialAdded(string Name, string PasswordHash, Guid Database);
