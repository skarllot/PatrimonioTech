using System.Text.Json.Serialization;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;

namespace PatrimonioTech.Domain.Credentials;

public sealed class UserCredential
{
    public const int DefaultKeySize = 512;
    public const int DefaultIterations = 100_000;

    [JsonConstructor]
    private UserCredential(
        string name,
        string salt,
        string key,
        Guid database,
        int keySize = DefaultKeySize,
        int iterations = DefaultIterations)
    {
        Name = name;
        Salt = salt;
        Key = key;
        Database = database;
        KeySize = keySize;
        Iterations = iterations;
    }

    public string Name { get; }
    public string Salt { get; }
    public string Key { get; }
    public Guid Database { get; }
    public int KeySize { get; }
    public int Iterations { get; }

    public static UserCredential Create(UserCredentialAdded added) =>
        new(added.Name, added.Salt, added.Key, added.Database, added.KeySize, added.Iterations);
}
