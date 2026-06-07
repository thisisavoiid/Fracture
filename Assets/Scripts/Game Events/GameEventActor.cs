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
        GameEventManager.Instance.Subscribe(listener, gameEvent);
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        GameEventManager.Instance.Unsubscribe(listener);
    }
}
