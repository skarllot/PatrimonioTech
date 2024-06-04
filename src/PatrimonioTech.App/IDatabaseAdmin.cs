namespace PatrimonioTech.App;

public interface IDatabaseAdmin
{
    Result<Unit, CreateDatabaseError> CreateDatabase(Guid fileId, string password);
}
