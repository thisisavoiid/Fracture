using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    private NavMeshAgent _agent;
    private Transform _targetTransform;
    private Battery _battery;
    private float _speed;
    private float _acceleration;
    public ChaseState(
        NavMeshAgent agent,
        Transform targetTransform,
        float speed,
        float acceleration,
        Battery battery
    )
    {
        _agent = agent;

        _speed = speed;
        _acceleration = acceleration;

        _targetTransform = targetTransform;
        _battery = battery;
    }

    public override void Enter()
    {
        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
    }
    public override void Exit()
    {
        _agent.ResetPath();
    }

    public override void Run()
    {
        if (_targetTransform == null)
            return;

        if (_agent == null)
            return;

        if (_battery != null && _agent.hasPath)
            _battery.Drain();

        _agent.SetDestination(_targetTransform.position);
    }
}