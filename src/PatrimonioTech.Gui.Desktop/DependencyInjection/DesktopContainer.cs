using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.DependencyInjection;
using PatrimonioTech.Infra.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop.DependencyInjection;

[ServiceProvider]
[Import<IAppModule>]
[Import<IInfraModule>]
[Singleton<ILoggerProvider, ConsoleLoggerProvider>]
[Singleton<IConfigureOptions<LoggerFilterOptions>>(Factory = nameof(ConfigureLoggerFilterOptions))]

// GUI
[Transient<App>]
[Singleton<IFactory<App>, ContainerFactory<App>>]

// Program
[Singleton<Program>]
public sealed partial class DesktopContainer(Action<LoggerFilterOptions>? loggerFilterOptions = null)
{
    private IConfigureOptions<LoggerFilterOptions> ConfigureLoggerFilterOptions() =>
        ILoggingModule.ConfigureFilterOptions(loggerFilterOptions);
}
