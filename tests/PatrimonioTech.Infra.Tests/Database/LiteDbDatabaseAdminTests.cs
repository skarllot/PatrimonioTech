using FluentAssertions;
using LiteDB;
using PatrimonioTech.App.SelfApplication;
using PatrimonioTech.Infra.Database;

namespace PatrimonioTech.Infra.Tests.Database;

public sealed class LiteDbDatabaseAdminTests : IDisposable
{
    private readonly string _tempDir;
    private readonly LiteDbDatabaseAdmin _sut;

    public LiteDbDatabaseAdminTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        _sut = new LiteDbDatabaseAdmin(new TestLocalPathProvider(_tempDir), new BsonMapper());
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    [Fact]
    public void CreateDatabase_WithValidInputs_ReturnsOk()
    {
        var result = _sut.CreateDatabase(Guid.NewGuid(), "testpassword123");

        result.Should().BeOk();
    }

    [Fact]
    public void CreateDatabase_WithValidInputs_CreatesFileAtExpectedPath()
    {
        var dbId = Guid.NewGuid();

        _sut.CreateDatabase(dbId, "testpassword123");

        var expectedPath = Path.Combine(_tempDir, $"{dbId:N}.db");
        File.Exists(expectedPath).Should().BeTrue();
    }

    [Fact]
    public void CreateDatabase_WithValidInputs_DatabaseIsAccessibleWithCorrectPassword()
    {
        var dbId = Guid.NewGuid();
        const string password = "testpassword123";

        _sut.CreateDatabase(dbId, password);

        var filePath = Path.Combine(_tempDir, $"{dbId:N}.db");
        var connectionString = new ConnectionString { Filename = filePath, Password = password };
        var act = () =>
        {
            using var db = new LiteDatabase(connectionString, new BsonMapper());
            db.Checkpoint();
        };
        act.Should().NotThrow();
    }

    private sealed class TestLocalPathProvider(string appData) : ILocalPathProvider
    {
        public string AppData => appData;
        public void Initialize() { }
    }
}
