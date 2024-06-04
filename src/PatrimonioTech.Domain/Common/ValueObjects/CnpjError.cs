namespace PatrimonioTech.Domain.Common.ValueObjects;

public enum CnpjError
{
    Empty = 1,
    TooShort,
    TooLong,
    Invalid,
}
