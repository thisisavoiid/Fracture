using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Cycles the agent through a sequence of predefined waypoints.
/// </summary>
public class PatrolState : State
{
    private NavMeshAgent _agent;
    private List<Vector3> _waypoints;
    private int _currentWaypointIdx;
    private Battery _battery;
    private float _speed;
    private float _acceleration;

    public PatrolState(
        NavMeshAgent agent,
        List<Vector3> waypoints,
        float speed,
        float acceleration,
        Battery battery
    )
    {
        _agent = agent;
        _speed = speed;
        _acceleration = acceleration;
        _waypoints = waypoints;
        _battery = battery;
        _currentWaypointIdx = 0;
    }

    /// <summary>
    /// Resets the path and starts movement toward the current waypoint index.
    /// </summary>
    public override void Enter()
    {
        if (_agent == null || _waypoints == null || _waypoints.Count == 0)
        {
            Debug.LogError("[PATROL STATE] Missing dependencies or waypoints -");
            return;
        }

        _agent.acceleration = _acceleration;
        _agent.speed = _speed;
        _agent.ResetPath();
        _agent.SetDestination(_waypoints[_currentWaypointIdx]);
    }

    public override void Exit() { }

    /// <summary>
    /// Handles waypoint switching logic and battery consumption.
    /// </summary>
    public override void Run()
    {
        if (_agent == null || _waypoints == null || _waypoints.Count == 0) return;

        if (_agent.hasPath)
            _battery.Drain();

        if (_agent.remainingDistance <= 0.25f && !_agent.hasPath)
        {
            _currentWaypointIdx = (_currentWaypointIdx + 1) % _waypoints.Count;

            Vector3 nextPos = _waypoints[_currentWaypointIdx];
            nextPos.y = _agent.transform.position.y;

            _agent.SetDestination(nextPos);
        }
    }
}