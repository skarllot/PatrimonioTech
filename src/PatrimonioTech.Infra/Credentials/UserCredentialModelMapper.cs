using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;

namespace PatrimonioTech.Infra.Credentials;

public static class UserCredentialModelMapper
{
    public static UserCredentialModel ToModel(this UserCredential userCredential) => new UserCredentialModel(
        userCredential.Name,
        userCredential.Salt,
        userCredential.Key,
        userCredential.Database,
        userCredential.KeySize,
        userCredential.Iterations);

    public static UserCredential ToEntity(this UserCredentialModel model) => UserCredential.Create(
        new UserCredentialAdded(
            model.Name,
            model.Salt,
            model.Key,
            model.Database,
            model.KeySize,
            model.Iterations));
}
