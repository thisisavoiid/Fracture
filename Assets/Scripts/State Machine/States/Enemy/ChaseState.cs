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

    public override void Enter(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {
        _agent.SetDestination(_targetTransform.position);
        _agent.transform.LookAt(_targetTransform.position);
    }
}