using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    private NavMeshAgent _agent;
    public PatrolState(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public override void Enter(GameObject gameObject)
    {
        _agent.ResetPath();

        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {

    }
}