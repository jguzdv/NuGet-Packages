# JGUZDV.Linq

Provides some extension functions for Linq:
- Hierarchy ascension
- Hierarchy descension (breath and depth first)
- OrderBy with support for choosing the direction as parameter

**Ascendants(AndSelf)**
This will recursively get all parents and yield return them in order of occurance (nearest parent or self first).

```csharp
var parentsAndSelf = Nodes.AscendantsAndSelf(x => x.Parent);
var parents = Nodes.Ascendants(x => x.Parent);
```

**Descendants(AndSelf)**
This will recursively enumerate all children in breath-first or depth-first order

```csharp
var childrenAndSelf = Nodes.DescendantsAndSelf(x => x.Children, ChildEnumerationStrategy.BreadthFirst);
var children = Nodes.Descendants(x => x.Children) // Depth-first is default
```


**OrderByExtension**
```csharp
var ordered = reverseSort 
    ? Unordered.OrderBy(x => x.Prop1).ThenByDescending(x => x.Prop2)
    : Unordered.OrderByDescending(x => x.Prop1).ThenBy(x => x.Prop2)

// Can be written as:
var ordered = Unordered
    .OrderBy(x => x.Prop1, reverseSort ? OrderDirection.Ascending : OrderDirection.Descending)
    .ThenBy(x => x.Prop2, reverseSort ? OrderDirection.Descending : OrderDirection.Ascending);

// which looks longer at first glance, but allows to dynamically change the sorting direction for many attributes
```