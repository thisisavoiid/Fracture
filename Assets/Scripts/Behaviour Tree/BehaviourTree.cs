using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    private Node _rootNode;

    private void Start()
    {
        SelectorNode selectorNode01 = new SelectorNode();
        SequenceNode sequenceNode01 = new SequenceNode();
    }

    private void Update()
    {
        _rootNode.Execute();
    }
}