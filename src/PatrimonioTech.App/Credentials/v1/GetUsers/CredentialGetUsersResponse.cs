using System.Collections.Immutable;

namespace PatrimonioTech.App.Credentials.v1.GetUsers;

public sealed record CredentialGetUsersResponse(ImmutableList<string> UserNames);
