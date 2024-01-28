using Generator.Equals;
using PatrimonioTech.Domain.Common.Parsers;

namespace PatrimonioTech.Domain.Credentials;

[Equatable]
public sealed partial class Password
{
    public const int PasswordMinLength = 8;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password, PasswordError> Create(string value)
    {
        return StringParser.NotNullOrWhitespace(value).ToResult(PasswordError.Empty)
            .Ensure(v => v.Length >= PasswordMinLength, PasswordError.TooShort)
            .Map(v => new Password(v));
    }
}

public enum PasswordError
{
    Empty = 1,
    TooShort
}
