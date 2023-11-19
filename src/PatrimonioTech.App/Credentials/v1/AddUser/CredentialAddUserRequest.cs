using System.ComponentModel.DataAnnotations;
using PatrimonioTech.Domain.Credentials;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

public sealed record CredentialAddUserRequest(
    [property: MinLength(3)] string Name,
    [property: MinLength(8)] string Password,
    [property: Range(128, 4096)] int KeySize = UserCredential.DefaultKeySize,
    [property: Range(1000, 100_000_000)] int Iterations = UserCredential.DefaultIterations);
