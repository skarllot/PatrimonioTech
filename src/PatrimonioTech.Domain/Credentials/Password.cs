using OneOf;
using PatrimonioTech.Domain.Common;

namespace PatrimonioTech.Domain.Credentials;

public sealed record Password
{
    public const int PasswordMinLength = 8;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Either<PasswordError, Password> Create(string value)
    {
        return from v in value.Pipe(
                v => v.IsNotNullOrWhitespace()
                    .ToEither<PasswordError>(() => new PasswordError.Empty()),
                v => v.HaveLength(l => l >= PasswordMinLength)
                    .ToEither<PasswordError>(() => new PasswordError.TooShort(v)))
            select new Password(v);
    }
}

[GenerateOneOf]
public partial class PasswordError : OneOfBase<PasswordError.Empty, PasswordError.TooShort>
{
    public sealed record Empty;

    public sealed record TooShort(string Password);
}
