public delegate void Behaviour();
public class ActionNode : Node
{
    private Behaviour _behaviour;
    public ActionNode(Behaviour behaviour)
    {
        _behaviour = behaviour;
    }
    public override Result Execute()
    {
        _behaviour.Invoke();
        return Result.Running;
    }
}