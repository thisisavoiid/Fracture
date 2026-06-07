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
    [SerializeField] private GameSettings _gameSettings;
    [SerializeField] private UnityEvent<GameEvent> _onGameEventStart;
    [SerializeField] private UnityEvent _onGameEventEnd;

    private static GameEventManager _instance;
    public static GameEventManager Instance => _instance;
    private Dictionary<IGameEventListener, GameEvent> _listeners = new();
    private GameEvent _currentEvent = null;
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
        }

        if (_gameSettings == null)
        {
            Debug.LogWarning($"[GAME EVENT MANAGER] No game settings asset assigned. Therefore, no game events based logic will execute. -");
            return;
        }

        _nextEventStartTime = GetRandomEventStartTime();
    }

    private List<GameEvent> GetValidGameEvents()
    {
        if (_gameSettings == null)
            return null;
        
        if (_gameSettings.ValidGameEvents == null || _gameSettings.ValidGameEvents.Count == 0)
            return null;
        
        return _gameSettings.ValidGameEvents;
    }

    public void Subscribe(IGameEventListener listener, GameEvent gameEvent)
    {
        if (_listeners.ContainsKey(listener))
        {
            Debug.Log($"[GAME EVENT MANAGER] Subscribe - Listener: {listener} | Already registered, skipping -");
            return;
        }

        if (gameEvent == null)
            return;

        _listeners.Add(listener, gameEvent);

        Debug.Log($"[GAME EVENT MANAGER] Subscribe - Listener: {listener} | EventType: {gameEvent} | Total: {_listeners.Count} -");
    }

    public void Unsubscribe(IGameEventListener listener)
    {
        if (!_listeners.ContainsKey(listener))
        {
            Debug.Log($"[GAME EVENT MANAGER] Unsubscribe - Listener: {listener} | Not found, skipping -");
            return;
        }

        _listeners.Remove(listener);

        Debug.Log($"[GAME EVENT MANAGER] Unsubscribe - Listener: {listener} | Removed | Remaining: {_listeners.Count} -");
    }

    private GameEvent GetRandomGameEvent()
    {
        if (_gameSettings == null)
            return null;

        List<GameEvent> validGameEvents = GetValidGameEvents();

        if (validGameEvents == null || validGameEvents.Count == 0)
        {
            Debug.LogWarning($"[GAME EVENT MANAGER] GetRandomGameEventType - No valid events configured, returning None -");
            return null;
        }

        return validGameEvents[Random.Range(0, validGameEvents.Count)];
    }

    private void StartRandomEvent()
    {
        _currentEvent = GetRandomGameEvent();
        Debug.Log($"[GAME EVENT MANAGER] StartRandomEvent - EventType: {_currentEvent} | EndTime: {_currentEventEndTime} -");
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.Start);
        _onGameEventStart?.Invoke(_currentEvent);
    }

    private void EndCurrentEvent()
    {
        Debug.Log($"[GAME EVENT MANAGER] EndCurrentEvent - EventType: {_currentEvent} | NextEventAt: {_nextEventStartTime} -");
        InvokeEventCallbacks(_currentEvent, GameEventCallbackType.End);
        _currentEvent = null;
        _onGameEventEnd?.Invoke();
    }

    private void Update()
    {
        if (_gameSettings == null)
            return;

        _ticks += Time.deltaTime;

        if (_ticks >= _nextEventStartTime && _currentEvent == null)
        {
            StartRandomEvent();
            _currentEventEndTime = GetRandomEventEndTime();
            Debug.Log($"[GAME EVENT MANAGER] Starting new random event with determined end time: {_currentEventEndTime} -");
        }

        if (_ticks >= _currentEventEndTime && _currentEvent != null)
        {
            EndCurrentEvent();
            _nextEventStartTime = GetRandomEventStartTime();
            Debug.Log($"[GAME EVENT MANAGER] Stopping current event ({_currentEvent.Name}) and proceeding with new event at determined time: {_nextEventStartTime} -");
        }
    }

    private float GetRandomEventStartTime()
    {
        if (_gameSettings == null)
            return 0.0f;

        return _ticks + Random.Range(_gameSettings.TimeBetweenGameEvents.Min, _gameSettings.TimeBetweenGameEvents.Max);
    }

    private float GetRandomEventEndTime()
    {
        if (_gameSettings == null)
            return 0.0f;

        return _ticks + Random.Range(_gameSettings.GameEventDuration.Min, _gameSettings.GameEventDuration.Max);
    }

    private void InvokeEventCallbacks(GameEvent gameEvent, GameEventCallbackType callbackType)
    {
        if (_listeners == null || _listeners.Count == 0)
        {
            Debug.Log($"[GAME EVENT MANAGER] InvokeEventCallbacks - No listeners registered, skipping -");
            return;
        }

        Debug.Log($"[GAME EVENT MANAGER] InvokeEventCallbacks - EventType: {gameEvent} | CallbackType: {callbackType} | Listeners: {_listeners.Count} -");

        Dictionary<IGameEventListener, GameEvent> listeners = _listeners;

        foreach (var listenerEntry in listeners)
        {
            if (listenerEntry.Value != gameEvent)
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