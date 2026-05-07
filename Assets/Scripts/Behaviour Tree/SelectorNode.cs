using Unity.VisualScripting;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    public SelectorNode()
    {
        _validResult = Result.Failure;
    }
}