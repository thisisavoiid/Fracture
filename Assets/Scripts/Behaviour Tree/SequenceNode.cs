using Unity.VisualScripting;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    public SequenceNode()
    {
        _validResult = Result.Success;
    }
}