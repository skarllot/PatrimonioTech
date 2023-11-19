﻿using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using PatrimonioTech.App.DependencyInjection;
using PatrimonioTech.Infra.DependencyInjection;

namespace PatrimonioTech.Gui.Desktop.DependencyInjection;

[ServiceProvider]
[Import<IAppModule>]
[Import<IInfraModule>]
[Singleton<ILoggerProvider>(Factory = nameof(CreateConsoleLoggerProvider))]
[Transient<LogLevel>(Factory = nameof(GetLogLevel))]

// GUI
[Transient<App>]
[Singleton<IFactory<App>, ContainerFactory<App>>]

// Program
[Singleton<Program>]
public sealed partial class DesktopContainer
{
    private static LogLevel GetLogLevel() => LogLevel.Information;

    private static ConsoleLoggerProvider CreateConsoleLoggerProvider(
        IOptionsMonitor<ConsoleLoggerOptions> options,
        IEnumerable<ConsoleFormatter> formatters) =>
        new ConsoleLoggerProvider(options, formatters);
}
