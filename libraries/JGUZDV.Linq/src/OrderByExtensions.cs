namespace System.Linq;

/// <summary>
/// Provides extension methods for ordering sequences with a specified direction.
/// </summary>
public static class OrderByExtensions
{
    /// <summary>
    /// Orders the elements of a sequence according to a key and a specified direction.
    /// </summary>
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

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence according to a key and a specified direction.
    /// </summary>
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

    /// <summary>
    /// Toggles the order direction between ascending and descending.
    /// </summary>
    public static OrderDirection Toggle(this OrderDirection s)
        => s == OrderDirection.Ascending
            ? OrderDirection.Descending
            : OrderDirection.Ascending;
}

/// <summary>
/// Specifies the direction of ordering for a sequence.
/// </summary>
public enum OrderDirection
{
    /// <summary>
    /// Orders the elements in ascending order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Orders the elements in descending order.
    /// </summary>
    Descending
}
