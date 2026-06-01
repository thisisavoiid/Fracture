using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameEventType
{
    None = -1,
    Rain = 0,
    LowGravity = 1,
    Attack = 2
}

public enum GameEventCallbackType
{
    Start,
    End
}

public interface IGameEventListener
{
    void Subscribe(IGameEventListener listener, GameEventType eventType);
    void Unsubscribe(IGameEventListener listener);
    public void EventStartCallback();
    public void EventEndCallback();
}

public class GameEventController : MonoBehaviour
{
    [SerializeField] private Range _timeRangeBetweenEvents;
    [SerializeField] private Range _timeRangeEventDuration;
    [SerializeField] private List<GameEventType> _validGameEvents;

    private static GameEventController _instance;
    public static GameEventController Instance => _instance;
    private Dictionary<IGameEventListener, GameEventType> _listeners = new();
    private GameEventType _currentEvent = GameEventType.None;
    private float _ticks = 0;
    private float _nextEventStartTime = 0;
    private float _currentEventEndTime = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Subscribe(IGameEventListener listener, GameEventType eventType)
    {
        if (_listeners.ContainsKey(listener))
            return;

        _listeners.Add(listener, eventType);

        Debug.Log($"[GAME EVENT CONTROLLER] {listener} Subscribed for event: {eventType}! -");
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        if (!_listeners.ContainsKey(listener))
            return;

        _listeners.Remove(listener);

        Debug.Log($"[GAME EVENT CONTROLLER] {listener} Got removed! -");
    }

    private GameEventType GetRandomGameEventType()
    {
        if (_validGameEvents == null || _validGameEvents.Count == 0)
            return GameEventType.None;

        return _validGameEvents[Random.Range(0, _validGameEvents.Count)];
    }

    private void StartRandomEvent()
    {
        _currentEvent = GetRandomGameEventType();
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.Start);
    }

    private void EndCurrentEvent()
    {
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.End);
        _currentEvent = GameEventType.None;
    }

    private void Update()
    {
        _ticks += Time.deltaTime;

        if (_ticks >= _nextEventStartTime && _currentEvent == GameEventType.None)
        {
            StartRandomEvent();
            _currentEventEndTime = _ticks + Random.Range(_timeRangeEventDuration.Min, _timeRangeEventDuration.Max);
        }

        if (_ticks >= _currentEventEndTime && _currentEvent != GameEventType.None)
        {
            EndCurrentEvent();
            _nextEventStartTime = _ticks + Random.Range(_timeRangeBetweenEvents.Min, _timeRangeBetweenEvents.Max);
        }
    }

    private void InvokeEventCallbacks(GameEventType eventType, GameEventCallbackType callbackType)
    {
        if (_listeners == null || _listeners.Count == 0)
            return;

        Dictionary<IGameEventListener, GameEventType> listeners = _listeners;

        foreach (var listenerEntry in listeners)
        {
            if (listenerEntry.Value != eventType)
                continue;

            IGameEventListener currentListener = listenerEntry.Key;

            switch (callbackType)
            {
                case GameEventCallbackType.Start:
                    currentListener.EventStartCallback();
                    break;

                case GameEventCallbackType.End:
                    currentListener.EventEndCallback();
                    break;
            }
        }
    }
}