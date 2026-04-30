using UnityEngine;
using UnityEngine.AI;

public class GoToChargeStationState : State
{
    private Transform _chargeStationTransform;
    private NavMeshAgent _agent;
    private float _speed;
    private float _acceleration;

    public GoToChargeStationState(
        Transform chargeStationTransform,
        NavMeshAgent agent,
        float speed,
        float acceleration
    )
    {
        _chargeStationTransform = chargeStationTransform;

        _agent = agent;

        _speed = speed;
        _acceleration = acceleration;
    }

    public override void Enter()
    {
        if (_chargeStationTransform == null)
            return;

        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
        _agent.SetDestination(_chargeStationTransform.position);
    }

    public override void Exit()
    {

    }

    public override void Run()
    {

    }
}