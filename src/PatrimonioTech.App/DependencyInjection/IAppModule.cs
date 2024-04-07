using Jab;
using PatrimonioTech.App.Credentials.v1.AddUser;
using PatrimonioTech.App.Credentials.v1.GetUserInfo;
using PatrimonioTech.App.Credentials.v1.GetUsers;

namespace PatrimonioTech.App.DependencyInjection;

[ServiceProviderModule]
[Scoped<ICredentialAddUserUseCase, CredentialAddUserUseCase>]
[Scoped<ICredentialGetUsersUseCase, CredentialGetUsersUseCase>]
[Scoped<ICredentialGetUserInfoUseCase, CredentialGetUserInfoUseCase>]
public interface IAppModule;
