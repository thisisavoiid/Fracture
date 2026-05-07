using UnityEngine;

public class CompositeNode : Node
{
    protected Result _validResult = Result.Undefined;

    public override Result Execute()
    {   
        return ExecuteComposite(_validResult);
    }

    private Result ExecuteComposite(Result validResult)
    {
        foreach (Node childNode in _children)
        {
            Result nodeResult = childNode.Execute();

            if (nodeResult != validResult)
                return Result.Failure;
        }

        return Result.Success;
    }
}