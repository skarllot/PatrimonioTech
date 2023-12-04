namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

public sealed record UserCredentialAdded(
    string Name,
    string Salt,
    string Key,
    Guid Database,
    int KeySize,
    int Iterations);
