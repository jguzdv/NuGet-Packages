using System.Diagnostics;

namespace System.Linq;

public static class HierarchyAscendingEnumerableExtensions
{
    public static IEnumerable<TSource> Ascendants<TSource>(
        this TSource startNode,
        Func<TSource, TSource?> parentSelector)
        => Ascendants(startNode, parentSelector, false);

    public static IEnumerable<TSource> AscendantsAndSelf<TSource>(
        this TSource startNode,
        Func<TSource, TSource?> parentSelector)
        => Ascendants(startNode, parentSelector, true);


    public static IEnumerable<TSource> Ascendants<TSource>(
        this TSource startNode,
        Func<TSource, TSource?> parentSelector,
        bool includeStartNode)
    {
        TSource? currentNode = startNode;
        if (includeStartNode)
            yield return currentNode;

        while((currentNode = parentSelector(currentNode)) != null)
            yield return currentNode;
    }
}
