using System.Collections.Generic;

public abstract class Node
{
    protected List<Node> _children = new();

    public void AddChild(Node node)
    {
        if (_children.Contains(node))
            return;

        _children.Add(node);
    }

    public abstract Result Execute();
}
