namespace PatrimonioTech.Domain.Common;

public static class PipeBindExtensions
{
    public static Option<T> Pipe<T>(
        this T obj,
        params Func<T, Option<T>>[] operators) =>
        operators.Length switch
        {
            0 => Some(obj),
            1 => operators[0](obj),
            _ => operators.Skip(1).Aggregate(operators[0](obj), static (e, o) => e.Bind(o))
        };

    public static Either<TL, TR> Pipe<TL, TR>(
        this TR obj,
        params Func<TR, Either<TL, TR>>[] operators) =>
        operators.Length switch
        {
            0 => Right(obj),
            1 => operators[0](obj),
            _ => operators.Skip(1).Aggregate(operators[0](obj), static (e, o) => e.Bind(o))
        };

    public static Either<TL, TR> Pipe<TL, TR>(
        this Either<TL, TR> either,
        params Func<TR, Either<TL, TR>>[] operators) =>
        operators.Aggregate(either, static (e, o) => e.Bind(o));
}
