using System.Reactive.Linq;

namespace PatrimonioTech.Gui.Common;

public static class ObservableExtensions
{
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
