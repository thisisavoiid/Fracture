using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    private NavMeshAgent _agent;
    private Transform _targetTransform;
    public ChaseState(NavMeshAgent agent, Transform targetTransform)
    {
        _agent = agent;
        _targetTransform = targetTransform;
    }

    public override void Enter() {}
    public override void Exit() {}

    public override void Run()
    {
        _agent.SetDestination(_targetTransform.position);
        _agent.transform.LookAt(_targetTransform.position);
    }
}