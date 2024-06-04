using FxKit.CompilerServices;

namespace PatrimonioTech.Domain.Credentials.Actions.AddUser;

[Union]
public partial record AddUserCredentialError
{
    partial record InvalidPassword(PasswordError Error);

    partial record NameTooShort;

    partial record KeySizeTooLow;

    partial record KeySizeTooHigh;

    partial record IterationsTooLow;

    partial record IterationsTooHigh;
}
