using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.DependencyInjection;
using PatrimonioTech.Domain.DependencyInjection;
using PatrimonioTech.Gui.DependencyInjection;
using PatrimonioTech.Gui.Login;
using PatrimonioTech.Infra.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop.DependencyInjection;

[ServiceProvider]
[Import<IDomainModule>]
[Import<IAppModule>]
[Import<IInfraModule>]
[Import<IGuiModule>]
[Singleton<ILoggerProvider, ConsoleLoggerProvider>]
[Singleton<IConfigureOptions<LoggerFilterOptions>>(Factory = nameof(ConfigureLoggerFilterOptions))]
public sealed partial class DesktopContainer(Action<LoggerFilterOptions>? loggerFilterOptions = null)
{
    private IConfigureOptions<LoggerFilterOptions> ConfigureLoggerFilterOptions() =>
        ILoggingModule.ConfigureFilterOptions(loggerFilterOptions);
}
