using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PatrimonioTech.Domain.Common;

[SuppressMessage("Design", "MA0016:Prefer using collection abstraction instead of implementation")]
public static class CollectionExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory) where TKey : notnull
    {
        ref var existingValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

        if (!exists)
            existingValue = valueFactory(key);

        return existingValue!;
    }

    public static TValue AddOrUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> addValueFactory,
        Func<TKey, TValue, TValue> updateValueFactory)
        where TKey : notnull
    {
        ref var existingValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

        existingValue = exists
            ? updateValueFactory(key, existingValue!)
            : addValueFactory(key);

        return existingValue;
    }

    public static TValue AddOrUpdate<TKey, TValue, TArg>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TArg, TValue> addValueFactory,
        Func<TKey, TValue, TArg, TValue> updateValueFactory,
        TArg factoryArgument) where TKey : notnull
    {
        ref var existingValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

        existingValue = exists
            ? updateValueFactory(key, existingValue!, factoryArgument)
            : addValueFactory(key, factoryArgument);

        return existingValue;
    }

    public static TValue AddOrUpdate<TKey, TValue, TArg>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TArg, TValue> addValueFactory,
        Action<TKey, TValue, TArg> updateValueFactory,
        TArg factoryArgument) where TKey : notnull
    {
        ref var existingValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);

        if (exists)
            updateValueFactory(key, existingValue!, factoryArgument);
        else
            existingValue = addValueFactory(key, factoryArgument);

        return existingValue!;
    }
}
