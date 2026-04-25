using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmPatrolState : State
{
    // public IdleState(NavMeshAgent agent)
    // {
    // }

    public override void Enter()
    {
        Debug.Log("Swarm patrol state enter");
    }

    public override void Exit() {}
    public override void Run( ) {}
}