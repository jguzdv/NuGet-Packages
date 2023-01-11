namespace System.Linq;

public static class OrderByExtensions
{
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        OrderDirection direction
    )
    {
        if (direction == OrderDirection.Ascending)
            return source.OrderBy(keySelector);
        else
            return source.OrderByDescending(keySelector);
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
        this IOrderedEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        OrderDirection direction
    )
    {
        if (direction == OrderDirection.Ascending)
            return source.ThenBy(keySelector);
        else
            return source.ThenByDescending(keySelector);
    }

    public static OrderDirection Toggle(this OrderDirection s)
        => s == OrderDirection.Ascending
            ? OrderDirection.Descending
            : OrderDirection.Ascending;
}

public enum OrderDirection
{
    Ascending,
    Descending
}
