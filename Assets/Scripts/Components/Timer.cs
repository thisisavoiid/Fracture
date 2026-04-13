using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _defaultTime;
    private float _timeLeft = 0.0f;
    private bool _isActive;

    public void Reset()
    {
        _timeLeft = _defaultTime;
    }

    public void SetTime(float time)
    {
        _defaultTime = time;
        Reset();
    }

    public void Start()
    {
        _isActive = true;
    }

    public void Stop()
    {
        _isActive = false;
    }
    
    public float GetRemainingTime()
    {
        return _timeLeft;
    }

    private void Update()
    {
        if (_timeLeft <= 0.0f)
        {
            _timeLeft = 0.0f;
            return;
        }

        if (!_isActive)
            return;

        _timeLeft -= Time.deltaTime;
    }
}