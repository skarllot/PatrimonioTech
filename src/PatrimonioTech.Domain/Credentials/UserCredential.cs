namespace PatrimonioTech.Domain.Credentials;

public sealed record UserCredential(
    string Name,
    string Salt,
    string Key,
    Guid Database,
    int KeySize = UserCredential.DefaultKeySize,
    int Iterations = UserCredential.DefaultIterations)
{
    public const int DefaultKeySize = 512;
    public const int DefaultIterations = 100_000;
}
