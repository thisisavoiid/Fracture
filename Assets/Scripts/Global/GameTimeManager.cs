using TMPro;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private MatchSettings _matchSettings;
    // [SerializeField] private ScriptableEvent _gameTimeEndEvent;

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
            _gameTimer.StopTimer();
    }

    private void OnEnable()
    {
        if (!_gameTimer.IsActive)
            _gameTimer.StartTimer();
    }

    private void Start()
    {
        if (!_gameTimer.IsActive)
            _gameTimer.StartTimer();
    }

}
