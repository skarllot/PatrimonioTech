namespace PatrimonioTech.Domain.Common;

public static class EitherLinqExtensions
{
    public static EitherAsync<TL, TV> SelectMany<TL, TR, TU, TV>(
        this Either<TL, TR> either,
        Func<TR, EitherAsync<TL, TU>> bind,
        Func<TR, TU, TV> project)
    {
        return either.Match(
            r => bind(r).Map(u => project(r, u)),
            l => l);
    }
}
