using FxKit.Parsers;
using Generator.Equals;

namespace PatrimonioTech.Domain.Common.ValueObjects;

[Equatable]
public sealed partial class NotEmptyString
{
    private NotEmptyString(string value) => Value = value;

    [CustomEquality(typeof(StringComparer), nameof(StringComparer.CurrentCultureIgnoreCase))]
    public string Value { get; }

    public static Result<NotEmptyString, NotEmptyStringError> Create(string value)
    {
        return from v in Optional(value).OkOr(NotEmptyStringError.Null)
            from parsed in StringParser.NonNullOrWhiteSpace(v).OkOr(NotEmptyStringError.Empty)
            select new NotEmptyString(parsed.Trim());
    }

    public override string ToString() => Value;
}
