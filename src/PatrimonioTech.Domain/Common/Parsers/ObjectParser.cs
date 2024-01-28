namespace PatrimonioTech.Domain.Common.Parsers;

public static class ObjectParser
{
    public static Maybe<T> NotNull<T>(T? value) where T : class => Maybe.From(value!);
}
