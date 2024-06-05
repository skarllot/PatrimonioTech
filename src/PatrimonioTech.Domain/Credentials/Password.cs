using FxKit.Parsers;
using Generator.Equals;

namespace PatrimonioTech.Domain.Credentials;

[Equatable]
public sealed partial class Password
{
    public const int PasswordMinLength = 8;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password, PasswordError> Create(string value)
    {
        return StringParser.NonNullOrWhiteSpace(value).OkOr(PasswordError.Empty)
            .Ensure(v => v.Length >= PasswordMinLength, PasswordError.TooShort)
            .Map(v => new Password(v));
    }
}
