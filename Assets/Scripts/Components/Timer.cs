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
    private float _timeSecondDelta = 0.0f;
    private bool _isActive;
    private bool _hasTimerEndEventInvoked = false;

    public void Reset()
    {
        Debug.Log($"[TIMER] Resetting timer (On object: {this.gameObject.name}), New Time: {_defaultTime.ToString()} -");
        _timeLeft = _defaultTime.TotalSeconds;
        _hasTimerEndEventInvoked = false;
        ResetTimerDelta();
    }

    public void SetTime(TimeMS time)
    {
        _defaultTime = time;
        Debug.Log($"[TIMER] SetTime called (On object: {this.gameObject.name}), Value: {time.ToString()}s -");
        Reset();
    }

    public void Start()
    {
        Debug.Log($"[TIMER] Starting timer (On object: {this.gameObject.name}) -");
        _isActive = true;
    }

    public void Stop()
    {
        Debug.Log($"[TIMER] Stopping timer (On object: {this.gameObject.name}) -");
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
                Debug.Log($"[TIMER] Timer reached zero (On object: {this.gameObject.name}), Invoking OnTimerEnd -");
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
        Debug.Log($"[TIMER] Interval Update (On object: {this.gameObject.name}), Time Left: {_timeLeft} -");
        _timeSecondDelta = 0.0f;
        OnTimerUpdate?.Invoke(new TimeMS(_timeLeft));
    }
}