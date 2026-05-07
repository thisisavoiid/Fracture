using System;

public class ConditionNode : Node
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override Result Execute()
    {
        bool isConditionMet = _condition();

        if (isConditionMet)
            return Result.Success;
        else
            return Result.Failure;
    }
}