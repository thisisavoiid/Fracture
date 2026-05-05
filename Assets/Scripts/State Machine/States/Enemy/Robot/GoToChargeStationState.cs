using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Directs the agent to a specific charging station location.
/// </summary>
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

    /// <summary>
    /// Initiates pathfinding to the station immediately upon entering the state.
    /// </summary>
    public override void Enter()
    {
        if (_chargeStationTransform == null)
            return;

        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
        _agent.SetDestination(_chargeStationTransform.position);
    }

    public override void Exit() { }

    public override void Run() { }
}