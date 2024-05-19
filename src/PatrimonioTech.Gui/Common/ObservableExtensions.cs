using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace PatrimonioTech.Gui.Common;

public static class ObservableExtensions
{
    public static IObservable<TResult> SwitchSelect<TSource, TResult>(
        this IObservable<TSource> source,
        Func<TSource, IObservable<TResult>> selector)
    {
        return source.Select(selector).Switch();
    }

    public static void Subscribe<TSource>(this IObservable<TSource> source, ICollection<IDisposable> disposables)
    {
        disposables.Add(source.Subscribe());
    }

    public static IObservable<TResult> Successes<TResult, TError>(this IObservable<Result<TResult, TError>> source)
    {
        return source.Where(x => x.IsSuccess).Select(x => x.Value);
    }

    public static IObservable<TError> Failures<TResult, TError>(this IObservable<Result<TResult, TError>> source)
    {
        return source.Where(x => x.IsFailure).Select(x => x.Error);
    }
}
