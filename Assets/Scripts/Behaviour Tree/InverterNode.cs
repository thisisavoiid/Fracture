public class InverterNode : Node
{
    public override Result Execute()
    {
        if (_children.Count <= 0)
            return Result.Undefined;

        Result result = _children[0].Execute();

        switch (result)
        {
            case Result.Success:
                return Result.Failure;

            case Result.Failure:
                return Result.Success;
        }

        return result;
    }
}