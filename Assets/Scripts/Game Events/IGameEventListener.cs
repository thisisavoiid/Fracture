public interface IGameEventListener
{
    void Subscribe(IGameEventListener listener, GameEvent eventType);
    void Unsubscribe(IGameEventListener listener);
    public void EventStartCallback();
    public void EventEndCallback();
}