namespace PatrimonioTech.Infra.Tests.Common;

public static class TheoryDataEnumerableExtensions
{
    public static TheoryData<T> ToTheoryData<T>(this IEnumerable<T> source)
    {
        var theoryData = new TheoryData<T>();
        foreach (T value in source)
        {
            theoryData.Add(value);
        }

        return theoryData;
    }

    public static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<(T1, T2)> source)
    {
        var theoryData = new TheoryData<T1, T2>();
        foreach (var value in source)
        {
            theoryData.Add(value.Item1, value.Item2);
        }

        return theoryData;
    }

    public static TheoryData<T1, T2, T3> ToTheoryData<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> source)
    {
        var theoryData = new TheoryData<T1, T2, T3>();
        foreach (var value in source)
        {
            theoryData.Add(value.Item1, value.Item2, value.Item3);
        }

        return theoryData;
    }
}
