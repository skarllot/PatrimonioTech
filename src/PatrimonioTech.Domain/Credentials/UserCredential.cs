using PatrimonioTech.Domain.Credentials.Actions.AddUser;

namespace PatrimonioTech.Domain.Credentials;

public sealed class UserCredential
{
    private UserCredential(string name, string passwordHash, Guid database)
    {
        Name = name;
        PasswordHash = passwordHash;
        Database = database;
    }

    public string Name { get; }
    public string PasswordHash { get; }
    public Guid Database { get; }

    public static UserCredential Create(UserCredentialAdded added) =>
        new(added.Name, added.PasswordHash, added.Database);
}
