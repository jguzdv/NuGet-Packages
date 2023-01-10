using System.Collections;
using Xunit;

namespace JGUZDV.Linq.Tests;

public class TreeNode : IEnumerable<TreeNode>
{
    public TreeNode(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public TreeNode? Parent { get; set; }
    public List<TreeNode> Children { get; } = new();

    public void Add(TreeNode node)
    {
        node.Parent = this;
        Children.Add(node);
    }

    public TreeNode this[int index] => Children[index];

    public override string ToString() => Name;

    public IEnumerator<TreeNode> GetEnumerator()
    {
        return ((IEnumerable<TreeNode>)Children).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Children).GetEnumerator();
    }
}


public class HierarchyTests
{
    private readonly List<TreeNode> TreeNodes;

    public HierarchyTests()
    {
        TreeNodes = new()
        {
            new("0")
            {
                new("0.0")
                {
                    new("0.0.0"),
                    new("0.0.1"),
                },
                new("0.1")
                {                        
                    new("0.1.0"),
                    new("0.1.1"),
                    new("0.1.2"),
                },
                new("0.2")
            },

            new("1")
            {
                new("1.0"),
                new("1.1"),
                new("2.2")
            },

            new("2")
        };
    }

    [Fact]
    public void Parent_Selection_Returns_All_ParentsAndSelf()
    {
        var parents = TreeNodes[0][1][2].AscendantsAndSelf(x => x.Parent);

        var actual = string.Join("#", parents);
        Assert.Equal("0.1.2#0.1#0", actual);
    }

    [Fact]
    public void Parent_Selection_Returns_All_Parents()
    {
        var parents = TreeNodes[0][1][2].Ascendants(x => x.Parent);

        var actual = string.Join("#", parents);
        Assert.Equal("0.1#0", actual);
    }

    [Fact]
    public void BFS_Strategy_Works()
    {
        var children = TreeNodes[0].Descendants(x => x.Children, ChildEnumerationStrategy.BreadthFirst);

        var actual = string.Join("#", children);
        Assert.Equal("0.0#0.1#0.2#0.0.0#0.0.1#0.1.0#0.1.1#0.1.2", actual);
    }

    [Fact]
    public void BFS_Strategy_Works_IncludesSelf()
    {
        var children = TreeNodes[0].DescendantsAndSelf(x => x.Children, ChildEnumerationStrategy.BreadthFirst);

        var actual = string.Join("#", children);
        Assert.Equal("0#0.0#0.1#0.2#0.0.0#0.0.1#0.1.0#0.1.1#0.1.2", actual);
    }

    [Fact]
    public void DFS_Strategy_Works()
    {
        var children = TreeNodes[0].Descendants(x => x.Children, ChildEnumerationStrategy.DepthFirst);

        var actual = string.Join("#", children);
        Assert.Equal("0.0#0.0.0#0.0.1#0.1#0.1.0#0.1.1#0.1.2#0.2", actual);
    }
    
    [Fact]
    public void DFS_Strategy_Works_IncludesSelf()
    {
        var children = TreeNodes[0].DescendantsAndSelf(x => x.Children, ChildEnumerationStrategy.DepthFirst);

        var actual = string.Join("#", children);
        Assert.Equal("0#0.0#0.0.0#0.0.1#0.1#0.1.0#0.1.1#0.1.2#0.2", actual);
    }
}
