using System.Runtime.CompilerServices;

namespace PatrimonioTech.Domain.Common;

public static class FunctionalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Apply<TValue, TError>(
        this Result<TValue, TError> result,
        Func<TValue, Maybe<TValue>> parse,
        TError error) =>
        result.TryGetValue(out TValue? value) ? parse(value).ToResult(error) : result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Ensure<TValue, TError>(
        this TValue value,
        Func<TValue, bool> predicate,
        TError error) =>
        predicate(value) ? value : error;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TOther, TError> Select<TValue, TOther, TError>(
        this Result<TValue, TError> result,
        Func<TValue, TOther> func) =>
        result.Map(func);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> ToResult<TValue, TError>(this Maybe<TValue> maybe, Func<TError> error) =>
        maybe.TryGetValue(out TValue? value) ? value : error();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Maybe<TValue> TryGetValue<TValue, TError>(this Result<TValue, TError> result) =>
        result.TryGetValue(out TValue? value) ? value : Maybe.None;
}
