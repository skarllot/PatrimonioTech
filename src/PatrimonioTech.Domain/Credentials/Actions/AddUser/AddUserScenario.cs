using Dunet;
using PatrimonioTech.Domain.Common;
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

    public Either<AddUserCredentialError, UserCredentialAdded> Execute(AddUserCredential command)
    {
        return from name in command.Name.Pipe(
                    n => n.IsNotNullOrWhitespace(),
                    n => n.HaveLength(l => l >= NameMinLength))
                .ToEither<AddUserCredentialError>(() => new AddUserCredentialError.NameTooShort(command.Name))
            from password in Password.Create(command.Password)
                .MapLeft<AddUserCredentialError>(e => new AddUserCredentialError.Password(e))
            from keySize in command.KeySize.Pipe(
                k => k.GreaterOrEqualsThan(KeySizeMinimum)
                    .ToEither<AddUserCredentialError>(() => new AddUserCredentialError.KeySizeTooLow(k)),
                k => k.LessOrEqualsThan(KeySizeMaximum)
                    .ToEither<AddUserCredentialError>(() => new AddUserCredentialError.KeySizeTooHigh(k)))
            from iterations in command.Iterations.Pipe(
                i => i.GreaterOrEqualsThan(IterationsMinimum)
                    .ToEither<AddUserCredentialError>(() => new AddUserCredentialError.IterationsTooLow(i)),
                i => i.LessOrEqualsThan(IterationsMaximum)
                    .ToEither<AddUserCredentialError>(() => new AddUserCredentialError.IterationsTooHigh(i)))
            let key = keyDerivation.CreateKey(password, keySize, iterations)
            let dbId = Guid.NewGuid()
            select new UserCredentialAdded(name, key.Salt, key.EncryptedKey, dbId, keySize, iterations);
    }
}

[Union]
public partial record AddUserCredentialError
{
    partial record Password(PasswordError Error);

    partial record NameTooShort(string Name);

    partial record KeySizeTooLow(int KeySize);

    partial record KeySizeTooHigh(int KeySize);

    partial record IterationsTooLow(int Iterations);

    partial record IterationsTooHigh(int Iterations);
}
