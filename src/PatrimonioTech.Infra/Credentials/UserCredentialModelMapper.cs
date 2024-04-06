using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using Riok.Mapperly.Abstractions;

namespace PatrimonioTech.Infra.Credentials;

[Mapper]
public static partial class UserCredentialModelMapper
{
    public static partial UserCredentialModel ToModel(this UserCredential userCredential);

    public static UserCredential ToEntity(this UserCredentialModel model) => UserCredential.Create(ToEvent(model));

    private static partial UserCredentialAdded ToEvent(UserCredentialModel model);
}
