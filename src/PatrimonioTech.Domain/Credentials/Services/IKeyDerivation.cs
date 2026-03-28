namespace PatrimonioTech.Domain.Credentials.Services;

public interface IKeyDerivation
{
    Result<IPhcString, CryptographyError> CreateKey(Password password);

    Result<string, GetKeyError> TryGetKey(string password, string passwordHash);
}
