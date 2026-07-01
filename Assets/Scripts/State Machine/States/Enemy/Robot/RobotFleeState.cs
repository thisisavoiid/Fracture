using System.IO;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;

public class RobotFleeState : State
{
    private NavMeshPointGenerator _pointGenerator;
    private NavMeshAgent _agent;
    private TransformVariable _targetTransform;
    private float _fleeFallbackRadius;
    private float _fleeDistance;
    private float _fleePositionReachedThreshold;
    private float _fleeSpeed;
    private Vector3 _fleePosition;

    public bool IsFleePositionReached => Vector3.Distance(
        _agent.transform.position, 
        _fleePosition
    ) <= _fleePositionReachedThreshold;

    public RobotFleeState(
        NavMeshPointGenerator pointGenerator,
        NavMeshAgent agent,
        TransformVariable targetTransform,
        float fleeFallbackRadius,
        float fleeDistance,
        float fleePositionReachedThreshold,
        float fleeSpeed
    )
    {
        _pointGenerator = pointGenerator;
        _agent = agent;
        _targetTransform = targetTransform;
        _fleeFallbackRadius = fleeFallbackRadius;
        _fleeDistance = fleeDistance;
        _fleePositionReachedThreshold = fleePositionReachedThreshold;
        _fleeSpeed = fleeSpeed;
    }

    public override void Enter()
    {
        Debug.Log($"[STATE MACHINE] Entering state: {this.GetType().Name.ToUpper()}.");

        if (_agent != null) 
            _agent.speed = _fleeSpeed;
            
        Vector3 fleeDirection = GetFleeDirection(
            _targetTransform.Value.position,
            _agent.transform.position
        );

        Vector3 fleePosition = GetSuitableFleePosition(
            fleeDirection,
            _fleeDistance
        );

        _fleePosition = fleePosition;

        _agent.SetDestination(fleePosition);
    }

    public override void Exit()
    {
        Debug.Log($"[STATE MACHINE] Exiting state: {this.GetType().Name.ToUpper()}.");
        _fleePosition = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        _agent.ResetPath();
    }

    public override void Run(float deltaTime)
    {
    }

    private Vector3 GetFleeDirection(Vector3 targetPos, Vector3 originPos)
    {
        return (originPos - targetPos).normalized;
    }

    private Vector3 GetSuitableFleePosition(Vector3 dir, float distance)
    {
        Vector3 roughTargetFleePosition = _agent.transform.position + dir * distance;

        bool hasFoundSuitableFleePosition = NavMesh.SamplePosition(
            roughTargetFleePosition,
            out NavMeshHit hit,
            distance,
            NavMesh.AllAreas
        );

        if (!hasFoundSuitableFleePosition)
        {
            Vector3 fallbackFleePosition = _pointGenerator.FindPosition(
                _fleeDistance
            );

            return fallbackFleePosition;
        }

        return hit.position;
    }
}