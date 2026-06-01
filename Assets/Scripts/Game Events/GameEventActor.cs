using UnityEngine;
using UnityEngine.Events;

public class GameEventActor : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEventType _eventType;
    [SerializeField] private UnityEvent _onEventStarted;
    [SerializeField] private UnityEvent _onEventEnded;

    private void Start()
    {
        Subscribe(this, _eventType);
    }

    // private void OnDisable()
    // {
    //     Unsubscribe(this);
    // }

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

    public void Subscribe(IGameEventListener listener, GameEventType eventType)
    {
        GameEventManager.Instance.Subscribe(this, _eventType);
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        GameEventManager.Instance.Unsubscribe(this);
    }
}
