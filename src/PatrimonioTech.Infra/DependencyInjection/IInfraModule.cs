using Jab;
using LiteDB;
using PatrimonioTech.App;
using PatrimonioTech.App.SelfApplication;
using PatrimonioTech.Domain.Credentials.Services;
using PatrimonioTech.Infra.Credentials.Services;
using PatrimonioTech.Infra.SelfApplication;
using Raiqub.JabModules.MicrosoftExtensionsOptions;

namespace PatrimonioTech.Infra.DependencyInjection;

[ServiceProviderModule]
[Import(typeof(IOptionsModule))]
[Import(typeof(ILoggingModule))]
[Singleton(typeof(IKeyDerivation), typeof(Pbkdf2KeyDerivation))]
[Scoped(typeof(IUserCredentialRepository), typeof(FileUserCredentialRepository))]
[Singleton(typeof(IDatabaseAdmin), typeof(LiteDbDatabaseAdmin))]
[Singleton(typeof(BsonMapper))]
[Singleton(typeof(ILocalPathProvider), typeof(LocalPathProvider))]
public interface IInfraModule;
