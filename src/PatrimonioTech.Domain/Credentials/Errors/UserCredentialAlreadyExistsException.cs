namespace PatrimonioTech.Domain.Credentials.Errors;

public sealed class UserCredentialAlreadyExistsException(string userName)
    : Exception($"The credential user name '{userName}' already exists");
