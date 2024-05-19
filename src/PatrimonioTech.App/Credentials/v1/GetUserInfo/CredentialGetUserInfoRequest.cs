namespace PatrimonioTech.App.Credentials.v1.GetUserInfo;

public sealed record CredentialGetUserInfoRequest(
    string UserName,
    string Password);
