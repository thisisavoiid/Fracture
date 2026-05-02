using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    private NavMeshAgent _agent;
    private List<Transform> _waypoints;
    private int _currentWaypointIdx;
    private Battery _battery;
    private float _speed;
    private float _acceleration;
    public PatrolState(
        NavMeshAgent agent,
        List<Transform> waypoints,
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
        Debug.Log("[PATROL STATE] PatrolState initialized -");
    }

    public override void Enter()
    {
        Debug.Log("[PATROL STATE] Entering state -");

        if (_agent == null)
        {
            Debug.LogError("[PATROL STATE] The NavMeshAgent is null -");
            return;
        }

        if (_waypoints == null || _waypoints.Count == 0)
        {
            Debug.LogError("[PATROL STATE] The waypoints list is null or empty -");
            return;
        }

        _agent.acceleration = _acceleration;
        _agent.speed = _speed;

        _agent.ResetPath();

        if (_waypoints != null && _waypoints.Count != 0)
            _agent.SetDestination(_waypoints[_currentWaypointIdx].position);

        Debug.Log($"[PATROL STATE] Destination set to waypoint index {_currentWaypointIdx} -");
    }

    public override void Exit()
    {
        Debug.Log("[PATROL STATE] Exiting state -");
    }

    public override void Run()
    {
        if (_agent == null || _waypoints == null || _waypoints.Count == 0) return;

        float distanceToCurrentWaypoint = (_waypoints[_currentWaypointIdx].position - _agent.transform.position).magnitude;

        if (_agent.hasPath)
            _battery.Drain();

        if (distanceToCurrentWaypoint <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath)
            {
                Debug.Log($"[PATROL STATE] Waypoint index {_currentWaypointIdx} reached -");

                _currentWaypointIdx++;
                if (_currentWaypointIdx >= _waypoints.Count)
                {
                    _currentWaypointIdx = 0;
                    Debug.Log("[PATROL STATE] Waypoint list looped back to index 0 -");
                }

                if (_waypoints[_currentWaypointIdx] == null)
                {
                    Debug.LogError($"[PATROL STATE] Waypoint at index {_currentWaypointIdx} is null -");
                    return;
                }

                Vector3 nextWaypoint = _waypoints[_currentWaypointIdx].position;
                nextWaypoint.y = _agent.transform.position.y;

                _agent.SetDestination(nextWaypoint);
                Debug.Log($"[PATROL STATE] Moving to next waypoint index {_currentWaypointIdx} -");
            }
        }
    }
}