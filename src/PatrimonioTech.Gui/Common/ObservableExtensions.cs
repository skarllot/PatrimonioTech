using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace PatrimonioTech.Gui.Common;

public static class ObservableExtensions
{
    /// <summary>
    /// Projects each element of an observable sequence to an observable sequence and
    /// transforms to a sequence producing values only from the most recent observable sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source observable sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements in the resulting observable sequence.</typeparam>
    /// <param name="source">The input observable sequence.</param>
    /// <param name="selector">A transform function to apply to each element in the source sequence.</param>
    /// <returns>An observable sequence that contains the elements from the projected observable sequences.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IObservable<TResult> SwitchSelect<TSource, TResult>(
        this IObservable<TSource> source,
        Func<TSource, IObservable<TResult>> selector)
    {
        return source.Select(selector).Switch();
    }

    /// <summary>
    /// Subscribes to the specified observable sequence and
    /// ensures that the subscription is disposed with the specified <paramref name="disposables"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the observable sequence.</typeparam>
    /// <param name="source">The input observable sequence.</param>
    /// <param name="disposables">A collection of disposables to manage subscriptions.</param>
    public static void Subscribe<TSource>(this IObservable<TSource> source, ICollection<IDisposable> disposables)
    {
        disposables.Add(source.Subscribe());
    }

    /// <summary>
    /// Subscribes to the specified observable sequence and
    /// ensures that the subscription is disposed with the specified <paramref name="disposables"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the observable sequence.</typeparam>
    /// <param name="source">The input observable sequence.</param>
    /// <param name="observer">The observer to subscribe.</param>
    /// <param name="disposables">A collection of disposables to manage subscriptions.</param>
    public static void Subscribe<TSource>(
        this IObservable<TSource> source,
        IObserver<TSource> observer,
        ICollection<IDisposable> disposables)
    {
        disposables.Add(source.Subscribe(observer));
    }

    /// <summary>
    /// Filters the successful results from an observable sequence of <see cref="Result{TOk,TErr}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input observable sequence of results.</param>
    /// <returns>An observable sequence containing the successful values.</returns>
    public static IObservable<TResult> Successes<TResult, TError>(this IObservable<Result<TResult, TError>> source)
        where TResult : notnull
        where TError : notnull
    {
        return source.Where(x => x.IsOk).Select(x => x.Unwrap());
    }

    /// <summary>
    /// Filters the error results from an observable sequence of <see cref="Result{TOk,TErr}"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the value in the result.</typeparam>
    /// <typeparam name="TError">The type of the error in the result.</typeparam>
    /// <param name="source">The input observable sequence of results.</param>
    /// <returns>An observable sequence containing the error values.</returns>
    public static IObservable<TError> Failures<TResult, TError>(this IObservable<Result<TResult, TError>> source)
        where TResult : notnull
        where TError : notnull
    {
        return source.Where(x => !x.IsOk).Select(x => x.UnwrapErr());
    }
}
