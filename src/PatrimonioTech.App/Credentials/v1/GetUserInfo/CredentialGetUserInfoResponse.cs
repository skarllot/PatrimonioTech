namespace PatrimonioTech.App.Credentials.v1.GetUserInfo;

public sealed record CredentialGetUserInfoResponse(
    string Name,
    Guid Database,
    string DatabasePassword);
