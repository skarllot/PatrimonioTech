using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PatrimonioTech.Infra.DependencyInjection;

[ServiceProviderModule]
[Import(typeof(IOptionsModule))]
[Singleton(typeof(ILoggerFactory), Factory = nameof(CreateLoggerFactory))]
[Singleton(typeof(ILogger<>), typeof(Logger<>))]
[Singleton(typeof(IConfigureOptions<LoggerFilterOptions>), Factory = nameof(CreateFilterOptions))]
public interface ILoggingModule
{
    public static IConfigureOptions<LoggerFilterOptions> CreateFilterOptions(LogLevel logLevel) =>
        IOptionsModule.Configure<LoggerFilterOptions>(options => options.MinLevel = logLevel);

    public static LoggerFactory CreateLoggerFactory(
        IEnumerable<ILoggerProvider> providers,
        IOptionsMonitor<LoggerFilterOptions> filterOption,
        IOptions<LoggerFactoryOptions> options) =>
        new LoggerFactory(providers, filterOption, options);
}
