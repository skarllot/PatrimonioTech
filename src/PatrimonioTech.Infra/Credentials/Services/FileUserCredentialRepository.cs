using System.Reactive;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.SelfApplication;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public partial class FileUserCredentialRepository : IUserCredentialRepository
{
    private readonly ILogger<FileUserCredentialRepository> _logger;
    private readonly string _appData;
    private readonly string _configFile;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileUserCredentialRepository(
        IOptions<FileUserCredentialOptions> options,
        ILogger<FileUserCredentialRepository> logger,
        ILocalPathProvider localPathProvider)
    {
        _logger = logger;
        _appData = localPathProvider.AppData;
        _configFile = Path.Combine(_appData, options.Value.FileName);
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
    }

    public async Task<Result<Unit, UserCredentialAddError>> Add(
        UserCredential userCredential,
        CancellationToken cancellationToken)
    {
        var current = await Read(cancellationToken).ConfigureAwait(false);

        if (current.Exists(u => u.Name.Equals(userCredential.Name, StringComparison.OrdinalIgnoreCase)))
        {
            LogUserAlreadyExists(userCredential.Name);
            return UserCredentialAddError.NameAlreadyExists;
        }

        current.Add(userCredential.ToModel());

        await Write(current, cancellationToken);

        LogNewUserAdded(userCredential.Name);
        return Unit.Default;
    }

    public async Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken)
    {
        var credentials = from m in await Read(cancellationToken).ConfigureAwait(false)
            select m.ToEntity();

        return credentials.ToList();
    }

    private async Task<List<UserCredentialModel>> Read(CancellationToken cancellationToken)
    {
        if (!File.Exists(_configFile))
        {
            await File
                .WriteAllTextAsync(_configFile, "[]", Encoding.UTF8, cancellationToken)
                .ConfigureAwait(false);
            return new List<UserCredentialModel>();
        }

        try
        {
            await using var fileStream = new FileStream(_configFile, FileMode.OpenOrCreate, FileAccess.Read);
            if (fileStream.Length == 0)
            {
                return new List<UserCredentialModel>();
            }

            var result = await JsonSerializer
                .DeserializeAsync<List<UserCredentialModel>>(fileStream, _jsonOptions, cancellationToken)
                .ConfigureAwait(false);
            return result ?? [];
        }
        catch (JsonException e)
        {
            LogInvalidConfigFile(e);

            File.Copy(_appData, $"{_appData}.{DateTime.UtcNow:yyyyMMddhhmmss}");
            await File
                .WriteAllTextAsync(_configFile, "[]", Encoding.UTF8, cancellationToken)
                .ConfigureAwait(false);
            LogConfigFileCreated();

            return [];
        }
    }

    private async Task Write(List<UserCredentialModel> data, CancellationToken cancellationToken)
    {
        await using var fileStream = new FileStream(_configFile, FileMode.Create, FileAccess.Write);
        await JsonSerializer
            .SerializeAsync(fileStream, data, _jsonOptions, cancellationToken)
            .ConfigureAwait(false);

        await fileStream
            .FlushAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    [LoggerMessage(LogLevel.Information, "The application configuration file was created")]
    private partial void LogConfigFileCreated();

    [LoggerMessage(LogLevel.Error, "The application configuration file is invalid")]
    private partial void LogInvalidConfigFile(Exception exception);

    [LoggerMessage(LogLevel.Warning, "The specified user name is already is use: {UserName}")]
    private partial void LogUserAlreadyExists(string userName);

    [LoggerMessage(LogLevel.Information, "A new user credential was added: {UserName}")]
    private partial void LogNewUserAdded(string userName);
}
