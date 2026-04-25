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

    public override void Enter()
    {
        _agent.ResetPath();
    }

    public override void Exit() {}
    public override void Run( ) {}
}