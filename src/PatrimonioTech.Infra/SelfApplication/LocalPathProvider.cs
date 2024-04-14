using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.SelfApplication;

namespace PatrimonioTech.Infra.SelfApplication;

public sealed partial class LocalPathProvider : ILocalPathProvider
{
    private readonly ILogger<LocalPathProvider> _logger;

    public LocalPathProvider(ILogger<LocalPathProvider> logger, IOptions<ApplicationOptions> options)
    {
        _logger = logger;
        AppData = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.Create),
            options.Value.Name);
    }

    public string AppData { get; }

    public void Initialize()
    {
        if (!Directory.Exists(AppData))
        {
            Directory.CreateDirectory(AppData);
            LogAppDataCreated();
        }
    }

    [LoggerMessage(LogLevel.Information, "The application data directory was created")]
    private partial void LogAppDataCreated();
}
