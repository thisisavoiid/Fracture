using System;
using UnityEngine;
using UnityEngine.Events;

public class GunContextEventListener : MonoBehaviour
{
    [Header("Event Configuration")]
    [Tooltip("The ScriptableEvent asset this component listens to.")]
    [SerializeField] private GunContextEvent _event;

    [Header("Responses")]
    [Tooltip("The UnityEvent that is invoked when the ScriptableEvent is triggered.")]
    [SerializeField] private UnityEvent<GunContext> _onEventInvoked;

    public void Notify(GunContext gunContext) => _onEventInvoked?.Invoke(gunContext);

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