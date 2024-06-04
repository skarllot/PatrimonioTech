using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PatrimonioTech.Domain.Common;

public static class FunctionalExtensions
{
    /// <summary>
    /// <para>Applies a parsing function to the value contained in a <see cref="Result{TOk,TErr}"/>.</para>
    /// <para>
    /// If the result has a value, the parsing function is called.
    /// If the parsing result is Some, it is converted to a result with its value.
    /// If the parsing result is None, a new result with the specified error is returned.
    /// </para>
    /// <para>Otherwise, the original result is returned.</para>
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input <see cref="Result{TValue, TError}"/>.</param>
    /// <param name="parse">The parsing function to apply to the value.</param>
    /// <param name="error">The default error value to return if parsing fails.</param>
    /// <returns>An <see cref="Result{TOk,TErr}"/> containing the parsed value or the original error.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Apply<TValue, TError>(
        this Result<TValue, TError> source,
        Func<TValue, Option<TValue>> parse,
        TError error)
        where TValue : notnull
        where TError : notnull =>
        source.TryGet(out var value, out _)
            ? parse(value).OkOrElse(error)
            : source;

    /// <summary>
    /// Retrieves either the value or the error from a <see cref="Result{TValue, TError}"/>.
    /// If the result is successful, the value is returned; otherwise, the error is returned.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="result">The input <see cref="Result{TValue, TError}"/>.</param>
    /// <returns>The value if successful, or the error if unsuccessful.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Case<TValue, TError>(this Result<TValue, TError> result)
        where TValue : notnull
        where TError : notnull =>
        result.TryGet(out var value, out var error) ? value : error;

    /// <summary>
    /// Chooses values from a sequence of <see cref="Option{T}"/> where the option has a value.
    /// Applies a selector function to each value and returns the results.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the option.</typeparam>
    /// <typeparam name="TResult">The type of the result after applying the selector function.</typeparam>
    /// <param name="source">The input sequence of <see cref="Option{T}"/>.</param>
    /// <param name="selector">The selector function to apply to each value.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the results of applying the selector function to the values.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TResult> Choose<TValue, TResult>(
        this IEnumerable<Option<TValue>> source,
        Func<TValue, TResult> selector)
        where TValue : notnull
    {
        foreach (var item in source)
        {
            if (item.TryGet(out var value))
                yield return selector(value);
        }
    }

    /// <summary>
    /// Chooses the values from a sequence of <see cref="Option{T}"/> where the option has a value.
    /// </summary>
    /// <typeparam name="T">The type of the value in the option.</typeparam>
    /// <param name="source">The input sequence of <see cref="Option{T}"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the values from the options with values.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Choose<T>(
        this IEnumerable<Option<T>> source)
        where T : notnull
    {
        foreach (var item in source)
        {
            if (item.TryGet(out var value))
                yield return value;
        }
    }

    /// <summary>
    /// Returns a new failure result if the predicate is false. Otherwise, returns the original result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input <see cref="Result{TOk,TErr}"/>.</param>
    /// <param name="predicate">The predicate to be evaluated on the value.</param>
    /// <param name="error">The default error value to return if the check fails.</param>
    /// <returns>A <see cref="Result{TOk,TErr}"/> containing either the original value or the specified error.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Ensure<TValue, TError>(
        this Result<TValue, TError> source,
        Func<TValue, bool> predicate,
        TError error)
        where TValue : notnull
        where TError : notnull =>
        source.TryGet(out var value, out _)
            ? predicate(value) ? source : Err<TValue, TError>(error)
            : source;

    /// <summary>
    /// Returns a new failure result if the predicate is false. Otherwise, returns the original result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input <see cref="Result{TOk,TErr}"/>.</param>
    /// <param name="predicate">The predicate to be evaluated on the value.</param>
    /// <param name="error">A function that provides the default error value to return if the check fails.</param>
    /// <returns>A <see cref="Result{TOk,TErr}"/> containing either the original value or the specified error.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Ensure<TValue, TError>(
        this Result<TValue, TError> source,
        Func<TValue, bool> predicate,
        Func<TError> error)
        where TValue : notnull
        where TError : notnull =>
        source.TryGet(out var value, out _)
            ? predicate(value) ? source : Err<TValue, TError>(error())
            : source;

    /// <summary>
    /// Returns a failure result if the predicate is false. Otherwise, returns a result with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="value">The input value.</param>
    /// <param name="predicate">The predicate to be evaluated on the value.</param>
    /// <param name="error">A function that provides the default error value to return if the check fails.</param>
    /// <returns>A <see cref="Result{TOk,TErr}"/> containing either the original value or the specified error.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> Ensure<TValue, TError>(
        this TValue value,
        Func<TValue, bool> predicate,
        Func<TError> error)
        where TValue : notnull
        where TError : notnull =>
        predicate(value) ? value : error();

    /// <summary>
    /// Returns a result containing the value from an <see cref="Option{T}"/> if it exists.
    /// Otherwise, returns a result with the specified error value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input <see cref="Option{T}"/>.</param>
    /// <param name="invalidValue">The default error value to return if the option is empty.</param>
    /// <returns>A <see cref="Result{TOk,TErr}"/> containing either the value or the specified error.</returns>
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TValue, TError> OkOrElse<TValue, TError>(
        this Option<TValue> source,
        TError invalidValue)
        where TValue : notnull
        where TError : notnull =>
        source.TryGet(out var value)
            ? Ok<TValue, TError>(value)
            : Err<TValue, TError>(invalidValue);

    /// <summary>
    /// Tries to find the first element in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The input sequence of elements.</param>
    /// <returns>An <see cref="Option{T}"/> containing the first element, or <see cref="Option{T}.None"/> if sequence is empty.</returns>
    public static Option<T> TryFirst<T>(this IEnumerable<T> source)
        where T : notnull
    {
        if (source is IList<T> list)
        {
            if (list.Count > 0)
            {
                return list[0];
            }
        }
        else
        {
            using var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current;
            }
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Tries to find the first element in the sequence that satisfies the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence.</typeparam>
    /// <param name="source">The input sequence of elements.</param>
    /// <param name="predicate">The predicate to evaluate on each element.</param>
    /// <returns>An <see cref="Option{T}"/> containing the first matching element, or <see cref="Option{T}.None"/> if no match is found.</returns>
    public static Option<T> TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        where T : notnull
    {
        foreach (var element in source)
        {
            if (predicate(element))
            {
                return element;
            }
        }

        return Option<T>.None;
    }
}
