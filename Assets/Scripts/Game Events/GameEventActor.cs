using UnityEngine;
using UnityEngine.Events;

public class GameEventActor : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEvent _gameEvent;
    [SerializeField] private UnityEvent _onEventStarted;
    [SerializeField] private UnityEvent _onEventEnded;

    private void Start()
    {
        if (_gameEvent == null)
            return;
        
        Subscribe(this, _gameEvent);
    }

    private void OnDisable()
    {
        Unsubscribe(this);
    }

    public void EventEndCallback()
    {
        _onEventEnded?.Invoke();
        Debug.Log($"[GAME EVENT ACTOR] {this.gameObject.name} is sleeping again! ");
    }

    public void EventStartCallback()
    {
        _onEventStarted?.Invoke();
        Debug.Log($"[GAME EVENT ACTOR] {this.gameObject.name} just woke up! ");
    }

    public void Subscribe(IGameEventListener listener, GameEvent gameEvent)
    {
        if (!IsGameEventManagerExistant())
            return;
            
        GameEventManager.Instance.Subscribe(listener, gameEvent);
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        if (!IsGameEventManagerExistant())
            return;
        
        GameEventManager.Instance.Unsubscribe(listener);
    }

    private bool IsGameEventManagerExistant() {
        bool exists = GameEventManager.Instance != null;

        if (!exists)
            Debug.LogWarning("[GAME EVENT ACTOR] There's no active GameEventManager instance in the current scene! -");

        return GameEventManager.Instance != null;
    }
}
