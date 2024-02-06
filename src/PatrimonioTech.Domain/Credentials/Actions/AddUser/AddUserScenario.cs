using OneOf;
using PatrimonioTech.Domain.Common;
using PatrimonioTech.Domain.Common.Parsers;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

[GenerateAutomaticInterface]
public class AddUserScenario(IKeyDerivation keyDerivation) : IAddUserScenario
{
    private const int NameMinLength = 3;
    private const int KeySizeMinimum = 128;
    private const int KeySizeMaximum = 4096;
    private const int IterationsMinimum = 1000;
    private const int IterationsMaximum = 100_000_000;

    public Result<UserCredentialAdded, AddUserCredentialError> Execute(AddUserCredential command)
    {
        return from name in StringParser.NotNullOrWhitespace(command.Name)
                .Where(v => v.Length >= NameMinLength)
                .ToResult(() => AddUserCredentialError.Other.NameTooShort)
                .MapError(e => (AddUserCredentialError)e)
            from password in Password.Create(command.Password)
                .MapError(e => (AddUserCredentialError)e)
            from keySize in command.KeySize
                .Ensure(v => v >= KeySizeMinimum, AddUserCredentialError.Other.KeySizeTooLow)
                .Ensure(v => v <= KeySizeMaximum, AddUserCredentialError.Other.KeySizeTooHigh)
                .MapError(e => (AddUserCredentialError)e)
            from iterations in command.Iterations
                .Ensure(v => v >= IterationsMinimum, AddUserCredentialError.Other.IterationsTooLow)
                .Ensure(v => v <= IterationsMaximum, AddUserCredentialError.Other.IterationsTooHigh)
                .MapError(e => (AddUserCredentialError)e)
            let key = keyDerivation.CreateKey(password, keySize, iterations)
            let dbId = Guid.NewGuid()
            select new UserCredentialAdded(name, key.Salt, key.EncryptedKey, dbId, keySize, iterations);
    }
}

[GenerateOneOf]
public partial class AddUserCredentialError : OneOfBase<
    PasswordError,
    AddUserCredentialError.Other>
{
    public enum Other
    {
        NameTooShort = 1,
        KeySizeTooLow,
        KeySizeTooHigh,
        IterationsTooLow,
        IterationsTooHigh
    }
}
