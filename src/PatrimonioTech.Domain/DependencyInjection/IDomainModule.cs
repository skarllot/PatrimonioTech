using Jab;
using PatrimonioTech.Domain.Credentials.Actions.AddUser;

namespace PatrimonioTech.Domain.DependencyInjection;

[ServiceProviderModule]
[Scoped<IAddUserScenario, AddUserScenario>]
public interface IDomainModule;
