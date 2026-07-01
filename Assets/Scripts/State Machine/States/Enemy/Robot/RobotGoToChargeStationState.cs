using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Directs the agent to a specific charging station location.
/// </summary>
public class RobotGoToChargeStationState : State
{
    private Vector3 _chargeStationPosition;
    private NavMeshAgent _agent;
    private float _speed;
    private float _acceleration;

    public RobotGoToChargeStationState(
        Vector3 chargeStationPosition,
        NavMeshAgent agent,
        float speed,
        float acceleration
    )
    {
        _chargeStationPosition = chargeStationPosition;
        _agent = agent;
        _speed = speed;
        _acceleration = acceleration;
    }

    /// <summary>
    /// Initiates pathfinding to the station immediately upon entering the state.
    /// </summary>
    public override void Enter()
    {
        if (_chargeStationPosition == null)
            return;

        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
        _agent.SetDestination(_chargeStationPosition);
    }

    public override void Exit()
    {
        _agent.ResetPath();
    }

    public override void Run(float deltaTime) {}
}