namespace PatrimonioTech.Infra.Credentials;

public sealed record UserCredentialModel(
    string Name,
    string Salt,
    string Key,
    Guid Database,
    int KeySize,
    int Iterations);
