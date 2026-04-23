using UnityEngine;

public abstract class State
{ 
    public abstract void Enter();
    public abstract void Run();
    public abstract void Exit();
}