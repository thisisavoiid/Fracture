using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the pursuit logic, directing the agent to track a moving target.
/// </summary>
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

    /// <summary>
    /// Configures agent movement parameters for high-speed pursuit.
    /// </summary>
    public override void Enter()
    {
        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
    }

    /// <summary>
    /// Clears the path on exit to prevent further movement toward the target.
    /// </summary>
    public override void Exit()
    {
        _agent.ResetPath();
    }

    /// <summary>
    /// Updates the target destination and drains battery during movement.
    /// </summary>
    public override void Run()
    {
        if (_targetTransform == null || _agent == null)
            return;

        if (_battery != null && _agent.hasPath)
            _battery.Drain();

        _agent.SetDestination(_targetTransform.position);
    }
}