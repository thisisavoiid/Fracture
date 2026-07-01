using System;

[Serializable]
public abstract class State
{ 
    public abstract void Enter();
    public abstract void Run(float deltaTime);
    public abstract void Exit();
}