namespace PatrimonioTech.Domain.Common;

public static class StringValidator
{
    public static Option<string> IsNotNullOrWhitespace(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? None : Some(value);

    public static Option<string> HaveLength(this string value, Func<int, bool> countPredicate) =>
        countPredicate(value.Length) ? Some(value) : None;
}
