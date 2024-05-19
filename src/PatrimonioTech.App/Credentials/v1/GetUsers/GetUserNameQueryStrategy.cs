using PatrimonioTech.Domain.Credentials;

namespace PatrimonioTech.App.Credentials.v1.GetUsers;

public static class GetUserNameQueryStrategy
{
    public static IEnumerable<string> Run(IEnumerable<UserCredential> credentials) => credentials
        .Select(c => c.Name)
        .Order(StringComparer.CurrentCultureIgnoreCase);
}
