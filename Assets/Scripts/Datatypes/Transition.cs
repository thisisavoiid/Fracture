using System;

public struct Transition
{
    public Transition(State state, Func<bool> condition)
    {
        Target = state;
        Condition = condition;
    }
    public State Target;
    public Func<bool> Condition;
}