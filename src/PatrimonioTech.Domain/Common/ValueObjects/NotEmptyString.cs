using Generator.Equals;
using PatrimonioTech.Domain.Common.Parsers;

namespace PatrimonioTech.Domain.Common.ValueObjects;

[Equatable]
public sealed partial class NotEmptyString
{
    private NotEmptyString(string value) => Value = value;

    [CustomEquality(typeof(StringComparer), nameof(StringComparer.CurrentCultureIgnoreCase))]
    public string Value { get; }

    public static Result<NotEmptyString, NotEmptyStringError> Create(string value)
    {
        return Optional(value).OkOrElse(NotEmptyStringError.Null)
            .Apply(StringParser.NotNullOrWhitespace, NotEmptyStringError.Empty)
            .Map(v => new NotEmptyString(v.Trim()));
    }

    public override string ToString() => Value;
}
