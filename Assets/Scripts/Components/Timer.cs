using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private TimeMS _defaultTime;
    public UnityEvent OnTimerEnd;
    public UnityEvent<TimeMS> OnTimerUpdate;
    public bool IsActive => _isActive;
    private float _timeLeft = 0.0f;
    private float _timeSecondDelta = 1.0f;
    private bool _isActive;
    private bool _hasTimerEndEventInvoked = false;

    public void Reset()
    {
        _timeLeft = _defaultTime.TotalSeconds;
        _hasTimerEndEventInvoked = false;
        ResetTimerDelta();
    }

    public void SetTime(TimeMS time)
    {
        _defaultTime = time;
        OnTimerUpdate?.Invoke(_defaultTime);
        
        _hasTimerEndEventInvoked = false;
        Reset();
    }

    public void StartTimer()
    {
        _isActive = true;
    }

    public void StopTimer()
    {
        _isActive = false;
    }

    public TimeMS GetRemainingTime()
    {
        return new TimeMS(_timeLeft);
    }

    private void Update()
    {
        if (_timeLeft <= 0.0f)
        {
            _timeLeft = 0.0f;

            if (!_hasTimerEndEventInvoked)
            {
                _hasTimerEndEventInvoked = true;
                OnTimerEnd?.Invoke();
            }

            return;
        }

        if (!_isActive)
            return;

        _timeLeft -= Time.deltaTime;
        _timeSecondDelta += Time.deltaTime;

        if (_timeSecondDelta >= 1.0f)
            ResetTimerDelta();
    }

    private void ResetTimerDelta()
    {
        _timeSecondDelta = 0.0f;
        OnTimerUpdate?.Invoke(new TimeMS(_timeLeft));
    }
}