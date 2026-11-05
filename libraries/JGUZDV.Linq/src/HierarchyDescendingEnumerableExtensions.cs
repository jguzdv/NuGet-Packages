namespace System.Linq;

/// <summary>
/// Provides extension methods for enumerating the descendants of a node in a hierarchy.
/// </summary>
public static class HierarchyDescendingEnumerableExtensions
{
    /// <summary>
    /// Enumerates the descendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="childrenSelector">A function to select the children of a node.</param>
    /// <param name="strategy">The strategy to use when enumerating child elements.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the descendants of the start node.</returns>
    public static IEnumerable<TSource> Descendants<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        ChildEnumerationStrategy strategy = ChildEnumerationStrategy.DepthFirst
        )
        => Descendants(startNode, childrenSelector, strategy, false);

    /// <summary>
    /// Enumerates the descendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="childrenSelector">A function to select the children of a node.</param>
    /// <param name="strategy">The strategy to use when enumerating child elements.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the descendants of the start node and the start node.</returns>
    public static IEnumerable<TSource> DescendantsAndSelf<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        ChildEnumerationStrategy strategy = ChildEnumerationStrategy.DepthFirst)
        => Descendants(startNode, childrenSelector, strategy, true);


    /// <summary>
    /// Enumerates the descendants of a node in a hierarchy.
    /// </summary>
    /// <typeparam name="TSource">The type of the nodes in the hierarchy.</typeparam>
    /// <param name="startNode">The node to start the enumeration from.</param>
    /// <param name="childrenSelector">A function to select the children of a node.</param>
    /// <param name="strategy">The strategy to use when enumerating child elements.</param>
    /// <param name="includeStartNode">A boolean value indicating whether to include the start node in the enumeration.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the descendants of the start node and optionally the start node.</returns>
    public static IEnumerable<TSource> Descendants<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        ChildEnumerationStrategy strategy,
        bool includeStartNode)
        => strategy switch
        {
            ChildEnumerationStrategy.BreadthFirst => BreadthFirstDescendants(startNode, childrenSelector, includeStartNode),
            ChildEnumerationStrategy.DepthFirst => DepthFirstDescendants(startNode, childrenSelector, includeStartNode),
            _ => throw new NotSupportedException()
        };



    private static IEnumerable<TSource> BreadthFirstDescendants<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        bool includeStartNode)
    {
        var result = new List<TSource>();
        if (includeStartNode)
            yield return startNode;

        var queue = new Queue<TSource>();
        EnqueueChildren(queue, startNode, childrenSelector);
        
        while(queue.Count > 0)
        {
            var child = queue.Dequeue();
            yield return child;

            EnqueueChildren(queue, child, childrenSelector);
        }

        static void EnqueueChildren(Queue<TSource> queue, TSource node, Func<TSource, IEnumerable<TSource>?> childrenSelector)
        {
            foreach (var child in node.Children(childrenSelector))
                queue.Enqueue(child);
        }
    }


    private static IEnumerable<TSource> DepthFirstDescendants<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        bool includeStartNode)
    {
        var result = Enumerable.Empty<TSource>();
        if (includeStartNode)
            yield return startNode;

        var children = childrenSelector(startNode);
        if (children == null)
            yield break;

        foreach (var child in children)
        {
            yield return child;
            foreach (var grandChild in child.DepthFirstDescendants(childrenSelector, false))
            { 
                yield return grandChild; 
            }
        }
    }


    private static IEnumerable<TSource> Children<TSource>(this TSource node, Func<TSource, IEnumerable<TSource>?> childrenSelector)
        => childrenSelector(node) ?? Enumerable.Empty<TSource>();
}