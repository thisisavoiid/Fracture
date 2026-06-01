using UnityEngine;

public class GameEventActor : MonoBehaviour, IGameEventListener
{
    [SerializeField] private GameEventType _eventType;

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
        Debug.Log($"[{this.gameObject.name}] Is sleeping again! ");
    }

    public void EventStartCallback()
    {
        Debug.Log($"[{this.gameObject.name}] Got woke up! ");
    }

    public void Subscribe(IGameEventListener listener, GameEventType eventType)
    {
        GameEventController.Instance.Subscribe(this, _eventType);
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        GameEventController.Instance.Unsubscribe(this);
    }
}
