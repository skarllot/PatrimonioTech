using FxKit.CompilerServices;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

[Union]
public partial record AddUserCredentialError
{
    partial record InvalidPassword(PasswordError Error);

    partial record NameTooShort;

    partial record KeyDerivationFailed(CryptographyError Error);
}
