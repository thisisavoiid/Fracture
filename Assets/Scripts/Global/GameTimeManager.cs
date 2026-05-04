using TMPro;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private MatchSettings _matchSettings;
    [SerializeField] private ScriptableEvent _gameTimeEndEvent;

    private Timer _gameTimer;

    private void Awake()
    {
        if (_matchSettings == null)
        {
            Debug.LogWarning("[GAME TIME MANAGER] No match settings object specified for this game object! -");
            return;
        }

        Debug.Log($"[GAME TIME MANAGER] Setting up match timer with time: {_matchSettings.MatchTime.ToString()} -");

        _gameTimer = GetComponent<Timer>();
        _gameTimer.SetTime(_matchSettings.MatchTime);
    }

    private void OnDisable()
    {
        if (_gameTimer.IsActive)
            _gameTimer.Stop();
    }

    private void OnEnable()
    {
        if (!_gameTimer.IsActive)
            _gameTimer.Start();
    }

    public void InvokeGameTimeEndedEvent()
    {
        if (_gameTimeEndEvent == null)
            return;

        Debug.Log($"[GAME TIME MANAGER] Invoking game time end event... -");

        _gameTimeEndEvent.Invoke();
    }
}
