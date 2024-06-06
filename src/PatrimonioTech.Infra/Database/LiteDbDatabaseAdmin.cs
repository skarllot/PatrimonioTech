using LiteDB;
using PatrimonioTech.App.Database;
using PatrimonioTech.App.SelfApplication;

namespace PatrimonioTech.Infra.Database;

public sealed class LiteDbDatabaseAdmin(ILocalPathProvider localPathProvider, BsonMapper bsonMapper)
    : IDatabaseAdmin
{
    public Result<Unit, CreateDatabaseError> CreateDatabase(Guid fileId, string password)
    {
        var filePath = Path.Combine(localPathProvider.AppData, $"{fileId:N}.db");
        var connectionString = new ConnectionString { Filename = filePath, Password = password };

        using var database = new LiteDatabase(connectionString, bsonMapper);
        database.Checkpoint();
        return Unit.Default;
    }
}
