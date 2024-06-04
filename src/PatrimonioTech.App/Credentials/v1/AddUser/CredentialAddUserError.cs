using FxKit.CompilerServices;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.App.Credentials.v1.AddUser;

[Union]
public partial record CredentialAddUserError
{
    partial record BusinessError(AddUserCredentialError Error);

    partial record StorageError(UserCredentialAddError Error);

    partial record CryptographyError(GetKeyError Error);

    partial record DatabaseError(CreateDatabaseError Error);
}
