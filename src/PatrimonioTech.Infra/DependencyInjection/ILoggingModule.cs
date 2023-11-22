using Jab;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PatrimonioTech.Infra.DependencyInjection;

[ServiceProviderModule]
[Import(typeof(IOptionsModule))]
[Singleton(typeof(ILoggerFactory), typeof(LoggerFactory))]
[Singleton(typeof(ILogger<>), typeof(Logger<>))]
public interface ILoggingModule
{
    public static IConfigureOptions<LoggerFilterOptions>
        ConfigureFilterOptions(Action<LoggerFilterOptions>? configure) =>
        IOptionsModule.Configure(configure);
}
