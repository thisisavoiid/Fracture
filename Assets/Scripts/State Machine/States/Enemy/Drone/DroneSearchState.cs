using UnityEngine;
using UnityEngine.AI;
public class DroneSearchState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private TransformVariable _targetTransform;
    private NavMeshAgent _agent;
    private Vector3 _lastSeenPos;
    private float _searchDuration;
    private float _searchTimePassed = 0f;
    private float _searchPositionReachedTreshold;
    public bool IsSearchTimeOver => _searchTimePassed >= _searchDuration;

    public DroneSearchState(
        DroneBrain brain,
        Rigidbody rb,
        TransformVariable targetTransform,
        TimeMS searchDuration,
        NavMeshAgent agent,
        float searchPositionReachedThreshold
    )
    {
        _brain = brain;
        _rb = rb;
        _targetTransform = targetTransform;
        _searchDuration = searchDuration.TotalSeconds;
        _agent = agent;
        _searchPositionReachedTreshold = searchPositionReachedThreshold;
    }

    public override void Enter()
    {
        Debug.Log("entering search state");
        _lastSeenPos = _targetTransform.Value.position;
        _agent.ResetPath();
        _agent.SetDestination(_lastSeenPos);
        _searchTimePassed = 0f;
    }

    public override void Exit()
    {
        _searchTimePassed = 0f;
        _agent.ResetPath();
        _brain.ResetRotation();
    }

    public override void Run(float deltaTime)
    {
        _brain.RotateTowardsTarget();
        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force);

        bool hasArrivedAtLastSeenPosition = Vector3.Distance(
            _agent.transform.position,
            _lastSeenPos
        ) <= _searchPositionReachedTreshold;

        if (!hasArrivedAtLastSeenPosition)
            return;

        if (IsSearchTimeOver)
            return;

        _searchTimePassed += deltaTime;
    }

}