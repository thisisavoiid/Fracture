using UnityEngine;
using UnityEngine.AI;
public class RobotSearchState : State
{
    private TransformVariable _targetTransform;
    private NavMeshAgent _agent;
    private Vector3 _lastSeenPos;
    private Animator _animator;
    private float _searchDuration;
    private float _searchTimePassed = 0f;
    private float _searchPositionReachedTreshold;
    public bool IsSearchTimeOver => _searchTimePassed >= _searchDuration;

    public RobotSearchState(
        TransformVariable targetTransform,
        TimeMS searchDuration,
        NavMeshAgent agent,
        Animator animator,
        float searchPositionReachedTreshold
    )
    {
        _targetTransform = targetTransform;
        _searchDuration = searchDuration.TotalSeconds;
        _agent = agent;
        _animator = animator;
        _searchPositionReachedTreshold = searchPositionReachedTreshold;
    }

    public override void Enter()
    {
        _lastSeenPos = _targetTransform.Value.position;
        _agent.ResetPath();
        _agent.SetDestination(_lastSeenPos);
        _searchTimePassed = 0f;
    }

    public override void Exit()
    {
        _agent.ResetPath();
        _animator.SetBool("IsSearching", false);
    }

    public override void Run(float deltaTime)
    {
        bool hasArrivedAtLastSeenPosition = Vector3.Distance(
            _agent.transform.position,
            _lastSeenPos
        ) <= _searchPositionReachedTreshold;

        if (!hasArrivedAtLastSeenPosition)
            return;
        
        if (IsSearchTimeOver)
            return;
        
        _animator.SetBool("IsSearching", true);

        _searchTimePassed += deltaTime;
    }

}