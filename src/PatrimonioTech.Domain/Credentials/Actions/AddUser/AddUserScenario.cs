using FxKit.Parsers;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

[GenerateAutomaticInterface]
public sealed class AddUserScenario(IKeyDerivation keyDerivation) : IAddUserScenario
{
    public const int NameMinLength = 3;

    public Result<UserCredentialAdded, AddUserCredentialError> Execute(AddUserCredential command)
    {
        return from name in StringParser.NonNullOrWhiteSpace(command.Name?.Trim())
                .Where(v => v.Length >= NameMinLength)
                .OkOrElse(AddUserCredentialError.NameTooShort.Of)
            from password in Password.Create(command.Password)
                .MapErr(AddUserCredentialError.InvalidPassword.Of)
            from phcString in keyDerivation.CreateKey(password)
                .MapErr(AddUserCredentialError.KeyDerivationFailed.Of)
            let dbId = Guid.NewGuid()
            select new UserCredentialAdded(name, phcString.Value, dbId);
    }
}
