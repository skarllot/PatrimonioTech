using FxKit.Parsers;
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
        return from name in StringParser.NonNullOrWhiteSpace(command.Name)
                .Where(v => v.Length >= NameMinLength)
                .OkOrElse(AddUserCredentialError.NameTooShort.Of)
            from password in Password.Create(command.Password)
                .MapErr(AddUserCredentialError.InvalidPassword.Of)
            from keySize in command.KeySize
                .Ensure(v => v >= KeySizeMinimum, AddUserCredentialError.KeySizeTooLow.Of)
                .Ensure(v => v <= KeySizeMaximum, AddUserCredentialError.KeySizeTooHigh.Of)
            from iterations in command.Iterations
                .Ensure(v => v >= IterationsMinimum, AddUserCredentialError.IterationsTooLow.Of)
                .Ensure(v => v <= IterationsMaximum, AddUserCredentialError.IterationsTooHigh.Of)
            let key = keyDerivation.CreateKey(password, keySize, iterations)
            let dbId = Guid.NewGuid()
            select new UserCredentialAdded(name, key.Salt, key.EncryptedKey, dbId, keySize, iterations);
    }
}
