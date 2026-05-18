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
    private Animator _animator;

    public SearchState(
        TransformVariable targetTransform,
        NavMeshAgent agent,
        Timer searchTimer,
        TimeMS searchDuration,
        Animator animator
    )
    {
        _targetTransform = targetTransform;
        _agent = agent;
        _searchTimer = searchTimer;
        _searchDuration = searchDuration;
        _animator = animator;
    }

    public override void Enter()
    {
        _searchTimer.StopTimer();
        _searchTimer.SetTime(_searchDuration);
        _hasSearchTimerBeenStarted = false;

        _lastSeenPosition = _targetTransform.Value.position;
    }

    public override void Exit()
    {
        _searchTimer.StopTimer();
        _searchTimer.Reset();
        _animator.SetBool("IsSearching", false);
    }

    public override void Run()
    {
        // Debug.Log($"{_searchTimer.GetRemainingTime().ToString()}\nTimer has been started already: {_hasSearchTimerBeenStarted}\nDistance To Target: {GetDistanceToLastSeenPosition()}");

        if (GetDistanceToLastSeenPosition() > 0.25f)
            _agent.SetDestination(_lastSeenPosition);
        else
        {
            if (!_hasSearchTimerBeenStarted)
            {
                _animator.SetBool("IsSearching", true);
                _hasSearchTimerBeenStarted = true;
                _searchTimer.StartTimer();
            }
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