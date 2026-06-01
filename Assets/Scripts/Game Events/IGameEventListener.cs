public interface IGameEventListener
{
    void Subscribe(IGameEventListener listener, GameEventType eventType);
    void Unsubscribe(IGameEventListener listener);
    public void EventStartCallback();
    public void EventEndCallback();
}