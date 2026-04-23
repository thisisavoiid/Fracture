using System;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableEventListener : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _event;
    [SerializeField] private UnityEvent _onEventInvoked;

    public void Notify() => _onEventInvoked?.Invoke();
    
    private void OnEnable()
    {
        if (_event == null)
            return;

        _event.AddListener(this);
    }

    private void OnDisable()
    {
        if (_event == null)
            return;

        _event.RemoveListener(this);
    }
}