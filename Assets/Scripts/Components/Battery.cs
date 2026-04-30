using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Battery : MonoBehaviour
{
    [SerializeField] private float _drainRatePerSecond;
    [SerializeField] private float _chargeRatePerSecond;
    public bool IsDrained => _currentBatteryLife == 0;
    public bool IsCharged => _currentBatteryLife == 100;
    public float CurrentLife => _currentBatteryLife;
    public UnityEvent<float> OnBatteryLifeUpdate;
    public UnityEvent OnBatteryDepleted;
    public UnityEvent OnBatteryChargedUp;
    private float _currentBatteryLife = 100;
    private BatteryState _currentBatteryState = BatteryState.Charged;

    private enum BatteryState
    {
        Charged,
        Drained
    }

    private void SetBatteryState(BatteryState state)
    {
        if (_currentBatteryState == state)
            return;

        _currentBatteryState = state;

        switch (_currentBatteryState)
        {
            case BatteryState.Drained:
                OnBatteryDepleted?.Invoke();
                break;

            case BatteryState.Charged:
                OnBatteryChargedUp?.Invoke();
                break;
        }

        Debug.Log($"[BATTERY] Battery state of {this.gameObject.name} has been set to: {_currentBatteryState} -");
    }

    public void Drain()
    {
        float targetBatteryLife = Mathf.Max(0, _currentBatteryLife - _drainRatePerSecond * Time.deltaTime);
        _currentBatteryLife = targetBatteryLife;

        OnBatteryLifeUpdate?.Invoke(_currentBatteryLife);

        if (_currentBatteryLife <= 0)
            SetBatteryState(BatteryState.Drained);
    }

    public void Charge()
    {
        float targetBatteryLife = Mathf.Min(100, _currentBatteryLife + _chargeRatePerSecond * Time.deltaTime);
        _currentBatteryLife = targetBatteryLife;

        OnBatteryLifeUpdate?.Invoke(_currentBatteryLife);

        if (_currentBatteryLife >= 100)
            SetBatteryState(BatteryState.Charged);
    }
}