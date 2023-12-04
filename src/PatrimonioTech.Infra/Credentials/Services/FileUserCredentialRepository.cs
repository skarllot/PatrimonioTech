using System.Text;
using System.Text.Json;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatrimonioTech.Domain.Credentials;
using PatrimonioTech.Domain.Credentials.Services;

namespace PatrimonioTech.Infra.Credentials.Services;

public partial class FileUserCredentialRepository : IUserCredentialRepository
{
    private readonly ILogger<FileUserCredentialRepository> _logger;
    private readonly string _appData;
    private readonly string _configFile;

    public FileUserCredentialRepository(
        IOptions<FileUserCredentialOptions> options,
        ILogger<FileUserCredentialRepository> logger)
    {
        _logger = logger;
        _appData = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.Create),
            options.Value.ApplicationName);

        _configFile = Path.Combine(_appData, options.Value.FileName);
    }

    public async Task<Either<UserCredentialAddError, Unit>> Add(UserCredential userCredential, CancellationToken cancellationToken)
    {
        var current = await Read(cancellationToken).ConfigureAwait(false);

        if (current.Exists(u => u.Name.Equals(userCredential.Name, StringComparison.OrdinalIgnoreCase)))
        {
            LogUserAlreadyExists(userCredential.Name);
            return new UserCredentialAddError.NameAlreadyExists(userCredential.Name);
        }

        current.Add(userCredential);

        await using var fileStream = new FileStream(_configFile, FileMode.Create, FileAccess.Write);
        await JsonSerializer
            .SerializeAsync(fileStream, current, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await fileStream
            .FlushAsync(cancellationToken)
            .ConfigureAwait(false);

        LogNewUserAdded(userCredential.Name);
        return Unit.Default;
    }

    public async Task<IReadOnlyList<UserCredential>> GetAll(CancellationToken cancellationToken)
    {
        return await Read(cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<UserCredential>> Read(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_appData))
        {
            Directory.CreateDirectory(_appData);
            LogAppDataCreated();
        }

        if (!File.Exists(_configFile))
        {
            await File
                .WriteAllTextAsync(_configFile, "[]", Encoding.UTF8, cancellationToken)
                .ConfigureAwait(false);
            return new List<UserCredential>();
        }

        try
        {
            await using var fileStream = new FileStream(_configFile, FileMode.OpenOrCreate, FileAccess.Read);
            if (fileStream.Length == 0)
            {
                return new List<UserCredential>();
            }

            var result = await JsonSerializer
                .DeserializeAsync<List<UserCredential>>(fileStream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return result ?? new List<UserCredential>();
        }
        catch (JsonException e)
        {
            LogInvalidConfigFile(e);

            File.Copy(_appData, $"{_appData}.{DateTime.UtcNow:yyyyMMddhhmmss}");
            await File
                .WriteAllTextAsync(_configFile, "[]", Encoding.UTF8, cancellationToken)
                .ConfigureAwait(false);
            LogConfigFileCreated();

            return new List<UserCredential>();
        }
    }

    [LoggerMessage(LogLevel.Information, "The application data directory was created")]
    private partial void LogAppDataCreated();

    [LoggerMessage(LogLevel.Information, "The application configuration file was created")]
    private partial void LogConfigFileCreated();

    [LoggerMessage(LogLevel.Error, "The application configuration file is invalid")]
    private partial void LogInvalidConfigFile(Exception exception);

    [LoggerMessage(LogLevel.Warning, "The specified user name is already is use: {UserName}")]
    private partial void LogUserAlreadyExists(string userName);

    [LoggerMessage(LogLevel.Information, "A new user credential was added: {UserName}")]
    private partial void LogNewUserAdded(string userName);
}
