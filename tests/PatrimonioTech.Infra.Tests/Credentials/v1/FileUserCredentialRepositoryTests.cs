using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.SelfApplication;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;

namespace PatrimonioTech.Infra.Tests.Credentials.v1;

public sealed class FileUserCredentialRepositoryTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FileUserCredentialRepository _sut;

    public FileUserCredentialRepositoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        _sut = new FileUserCredentialRepository(
            Options.Create(new FileUserCredentialOptions()),
            NullLogger<FileUserCredentialRepository>.Instance,
            new TestLocalPathProvider(_tempDir));
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    [Fact]
    public async Task GetAll_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        var result = await _sut.GetAll(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Add_WithNewCredential_ReturnsOk()
    {
        var result = await _sut.Add(MakeCredential("alice"), CancellationToken.None);

        result.Should().BeOk();
    }

    [Fact]
    public async Task Add_WithNewCredential_CanBeRetrievedByGetAll()
    {
        await _sut.Add(MakeCredential("alice"), CancellationToken.None);

        var credentials = await _sut.GetAll(CancellationToken.None);

        credentials.Should().HaveCount(1);
        credentials[0].Name.Should().Be("alice");
    }

    [Fact]
    public async Task Add_WithMultipleCredentials_AllCanBeRetrievedByGetAll()
    {
        await _sut.Add(MakeCredential("alice"), CancellationToken.None);
        await _sut.Add(MakeCredential("bob"), CancellationToken.None);

        var credentials = await _sut.GetAll(CancellationToken.None);

        credentials.Should().HaveCount(2);
        credentials.Select(c => c.Name).Should().BeEquivalentTo(["alice", "bob"]);
    }

    [Fact]
    public async Task Add_WithDuplicateNameSameCase_ReturnsNameAlreadyExists()
    {
        await _sut.Add(MakeCredential("alice"), CancellationToken.None);

        var result = await _sut.Add(MakeCredential("alice"), CancellationToken.None);

        result.Should().BeErr().Should().Be(UserCredentialAddError.NameAlreadyExists);
    }

    [Fact]
    public async Task Add_WithDuplicateNameDifferentCase_ReturnsNameAlreadyExists()
    {
        await _sut.Add(MakeCredential("alice"), CancellationToken.None);

        var result = await _sut.Add(MakeCredential("ALICE"), CancellationToken.None);

        result.Should().BeErr().Should().Be(UserCredentialAddError.NameAlreadyExists);
    }

    [Fact]
    public async Task Add_WithDuplicateName_DoesNotAddNewEntry()
    {
        await _sut.Add(MakeCredential("alice"), CancellationToken.None);
        await _sut.Add(MakeCredential("ALICE"), CancellationToken.None);

        var credentials = await _sut.GetAll(CancellationToken.None);

        credentials.Should().HaveCount(1);
    }

    private static UserCredential MakeCredential(string name) =>
        UserCredential.Create(
            new UserCredentialAdded(name, "$pbkdf2-sha512-aes256cbc$i=100000,l=512$salt$key", Guid.NewGuid()));

    private sealed class TestLocalPathProvider(string appData) : ILocalPathProvider
    {
        public string AppData => appData;
        public void Initialize() { }
    }
}
