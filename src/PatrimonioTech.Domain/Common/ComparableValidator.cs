namespace PatrimonioTech.Domain.Common;

public static class ComparableValidator
{
    public static Option<T> LessThan<T>(this T value, T threshold) where T : IComparable<T>
        => value.CompareTo(threshold) switch
        {
            < 0 => Some(value),
            _ => None
        };

    public static Option<T> LessOrEqualsThan<T>(this T value, T threshold) where T : IComparable<T>
        => value.CompareTo(threshold) switch
        {
            <= 0 => Some(value),
            _ => None
        };

    public static Option<T> GreaterThan<T>(this T value, T threshold) where T : IComparable<T>
        => value.CompareTo(threshold) switch
        {
            > 0 => Some(value),
            _ => None
        };

    public static Option<T> GreaterOrEqualsThan<T>(this T value, T threshold) where T : IComparable<T>
        => value.CompareTo(threshold) switch
        {
            >= 0 => Some(value),
            _ => None
        };
}
