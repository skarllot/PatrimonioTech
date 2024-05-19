using System.Reactive;
using CSharpFunctionalExtensions;

namespace PatrimonioTech.App;

public interface IDatabaseAdmin
{
    Result<Unit, CreateDatabaseError> CreateDatabase(Guid fileId, string password);
}

public enum CreateDatabaseError;
