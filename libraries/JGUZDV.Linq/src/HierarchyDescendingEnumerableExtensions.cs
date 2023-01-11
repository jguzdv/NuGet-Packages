namespace System.Linq;

public static class HierarchyDescendingEnumerableExtensions
{
    public static IEnumerable<TSource> Descendants<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        ChildEnumerationStrategy strategy = ChildEnumerationStrategy.DepthFirst
        )
        => Descendants(startNode, childrenSelector, strategy, false);

    public static IEnumerable<TSource> DescendantsAndSelf<TSource>(
        this TSource startNode,
        Func<TSource, IEnumerable<TSource>?> childrenSelector,
        ChildEnumerationStrategy strategy = ChildEnumerationStrategy.DepthFirst)
        => Descendants(startNode, childrenSelector, strategy, true);


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