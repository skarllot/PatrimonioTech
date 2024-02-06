using PatrimonioTech.Domain.Credentials;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

public sealed record CredentialAddUserRequest(
    string Name,
    string Password,
    int KeySize = UserCredential.DefaultKeySize,
    int Iterations = UserCredential.DefaultIterations);
