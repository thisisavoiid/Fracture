using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameEventManager : MonoBehaviour
{
    [SerializeField] private Range _timeRangeBetweenEvents;
    [SerializeField] private Range _timeRangeEventDuration;
    [SerializeField] private List<GameEventType> _validGameEvents;
    [SerializeField] private UnityEvent _onGameEventStart;
    [SerializeField] private UnityEvent _onGameEventEnd;

    private static GameEventManager _instance;
    public static GameEventManager Instance => _instance;
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
        {
            Debug.Log($"[GAME EVENT CONTROLLER] Subscribe - Listener: {listener} | Already registered, skipping -");
            return;
        }

        _listeners.Add(listener, eventType);

        Debug.Log($"[GAME EVENT CONTROLLER] Subscribe - Listener: {listener} | EventType: {eventType} | Total: {_listeners.Count} -");
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        if (!_listeners.ContainsKey(listener))
        {
            Debug.Log($"[GAME EVENT CONTROLLER] Unsubscribe - Listener: {listener} | Not found, skipping -");
            return;
        }

        _listeners.Remove(listener);

        Debug.Log($"[GAME EVENT CONTROLLER] Unsubscribe - Listener: {listener} | Removed | Remaining: {_listeners.Count} -");
    }

    private GameEventType GetRandomGameEventType()
    {
        if (_validGameEvents == null || _validGameEvents.Count == 0)
        {
            Debug.LogWarning($"[GAME EVENT CONTROLLER] GetRandomGameEventType - No valid events configured, returning None -");
            return GameEventType.None;
        }

        return _validGameEvents[Random.Range(0, _validGameEvents.Count)];
    }

    private void StartRandomEvent()
    {
        _currentEvent = GetRandomGameEventType();
        Debug.Log($"[GAME EVENT CONTROLLER] StartRandomEvent - EventType: {_currentEvent} | EndTime: {_currentEventEndTime} -");
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.Start);
        _onGameEventStart?.Invoke();
    }

    private void EndCurrentEvent()
    {
        Debug.Log($"[GAME EVENT CONTROLLER] EndCurrentEvent - EventType: {_currentEvent} | NextEventAt: {_nextEventStartTime} -");
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.End);
        _currentEvent = GameEventType.None;
        _onGameEventEnd?.Invoke();
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
        {
            Debug.Log($"[GAME EVENT CONTROLLER] InvokeEventCallbacks - No listeners registered, skipping -");
            return;
        }

        Debug.Log($"[GAME EVENT CONTROLLER] InvokeEventCallbacks - EventType: {eventType} | CallbackType: {callbackType} | Listeners: {_listeners.Count} -");

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