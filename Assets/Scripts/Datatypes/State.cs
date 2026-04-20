using UnityEngine;

public abstract class State : ScriptableObject
{ 
    public abstract void Enter(StateMachineController controller);
    public abstract void Run(StateMachineController controller);
    public abstract void Exit(StateMachineController controller);
}