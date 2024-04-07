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

    public static IEnumerable<TResult> ToEnumerableSuccess<TResult, TError>(this Result<TResult, TError> result)
    {
        return result.TryGetValue(out var value) ? new[] { value } : [];
    }
}
