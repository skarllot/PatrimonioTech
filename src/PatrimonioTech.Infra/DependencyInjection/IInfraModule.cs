using Jab;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;
using Raiqub.JabModules.MicrosoftExtensionsOptions;

namespace PatrimonioTech.Infra.DependencyInjection;

[ServiceProviderModule]
[Import(typeof(IOptionsModule))]
[Import(typeof(ILoggingModule))]
[Singleton(typeof(IKeyDerivation), typeof(Pbkdf2KeyDerivation))]
[Scoped(typeof(IUserCredentialRepository), typeof(FileUserCredentialRepository))]
public interface IInfraModule;
