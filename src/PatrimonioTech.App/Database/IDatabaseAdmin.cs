namespace PatrimonioTech.App.Database;

public interface IDatabaseAdmin
{
    Result<Unit, CreateDatabaseError> CreateDatabase(Guid fileId, string password);
}
