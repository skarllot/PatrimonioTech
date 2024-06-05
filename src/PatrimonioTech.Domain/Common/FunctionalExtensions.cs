using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PatrimonioTech.Domain.Common;

public static class FunctionalExtensions
{
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
    public static Result<TValue, TError> OkOr<TValue, TError>(
        this Option<TValue> source,
        TError invalidValue)
        where TValue : notnull
        where TError : notnull =>
        source.TryGet(out var value)
            ? Ok<TValue, TError>(value)
            : Err<TValue, TError>(invalidValue);
}
