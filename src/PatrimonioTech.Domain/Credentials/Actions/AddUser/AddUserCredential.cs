namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

public sealed record AddUserCredential(
    string Name,
    string Password,
    int KeySize,
    int Iterations);
