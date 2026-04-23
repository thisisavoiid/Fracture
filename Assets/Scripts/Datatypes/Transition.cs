using System;

public struct Transition
{
    public Transition(State state, Func<bool> condition)
    {
        _targetState = state;
        _condition = condition;
    }
    private State _targetState;
    public State TargetState => _targetState;
    private Func<bool> _condition;
    public Func<bool> Condition => _condition;
}