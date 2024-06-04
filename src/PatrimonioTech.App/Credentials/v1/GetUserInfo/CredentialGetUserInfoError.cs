using FxKit.CompilerServices;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.GetUserInfo;

[Union]
public partial record CredentialGetUserInfoError
{
    partial record CryptographyError(GetKeyError Error);

    partial record Unexpected;

    partial record UserNotFound;
}
