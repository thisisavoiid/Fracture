using UnityEngine;
using UnityEngine.AI;

public class SearchState : State
{
    private Vector3 _lastSeenPosition;
    private NavMeshAgent _agent;
    private TimeMS _searchDuration;
    private Timer _searchTimer;
    private TransformVariable _targetTransform;
    private bool _hasSearchTimerBeenStarted = false;

    public SearchState(
        TransformVariable targetTransform,
        NavMeshAgent agent,
        Timer searchTimer,
        TimeMS searchDuration
    )
    {
        _targetTransform = targetTransform;
        _agent = agent;
        _searchTimer = searchTimer;
        _searchDuration = searchDuration;
    }

    public override void Enter()
    {
        _searchTimer.Stop();
        _searchTimer.SetTime(_searchDuration);

        _lastSeenPosition = _targetTransform.Value.position;
    }

    public override void Exit()
    {
        _searchTimer.Stop();
        _searchTimer.Reset();
        _hasSearchTimerBeenStarted = false;
    }

    public override void Run()
    {
        if (GetDistanceToLastSeenPosition() > _agent.stoppingDistance)
        {
            _agent.SetDestination(_lastSeenPosition);
            return;
        }
            
        if (!_hasSearchTimerBeenStarted)
        {
            _hasSearchTimerBeenStarted = true;
            _searchTimer.Start();
        }   
    }

    private float GetDistanceToLastSeenPosition()
    {
        Vector3 currAgentPosition = _agent.transform.position;
        currAgentPosition.y = 0.0f;

        Vector3 lastSeenPosition = _lastSeenPosition;
        lastSeenPosition.y = 0.0f;

        return (lastSeenPosition - currAgentPosition).magnitude;
    }
}