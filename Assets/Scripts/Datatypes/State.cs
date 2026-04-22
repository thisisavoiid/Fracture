using UnityEngine;

public abstract class State
{ 
    public abstract void Enter(GameObject gameObject);
    public abstract void Run(GameObject gameObject);
    public abstract void Exit(GameObject gameObject);
}