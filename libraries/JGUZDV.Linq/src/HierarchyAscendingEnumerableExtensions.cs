namespace System.Linq;

/// <summary>
/// Provides extension methods for enumerating the ascendants of a node in a hierarchy.
/// </summary>
public static class HierarchyAscendingEnumerableExtensions
{
    
    /// <summary>
    /// Enumerates the ascendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="parentSelector">A function to select the parent of a node.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the ascendants of the start node.</returns>
    public static IEnumerable<TSource> Ascendants<TSource>(
        this TSource startNode,
        Func<TSource, TSource?> parentSelector)
        => Ascendants(startNode, parentSelector, false);


    /// <summary>
    /// Enumerates the ascendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="parentSelector">A function to select the parent of a node.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the ascendants of the start node and the start node.</returns>
    public static IEnumerable<TSource> AscendantsAndSelf<TSource>(
        this TSource startNode,
        Func<TSource, TSource?> parentSelector)
        => Ascendants(startNode, parentSelector, true);

    /// <summary>
    /// Enumerates the ascendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="parentSelector">A function to select the parent of a node.</param>
    /// <param name="includeStartNode">A boolean value indicating whether to include the start node in the enumeration.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the ascendants of the start node and optionally the start node.</returns>
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
