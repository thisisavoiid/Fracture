using System;
using UnityEngine;
using UnityEngine.Events;

public class GunConfigEventListener : MonoBehaviour
{
    [Header("Event Configuration")]
    [Tooltip("The ScriptableEvent asset this component listens to.")]
    [SerializeField] private GunConfigEvent _event;

    [Header("Responses")]
    [Tooltip("The UnityEvent that is invoked when the ScriptableEvent is triggered.")]
    [SerializeField] private UnityEvent<GunConfig> _onEventInvoked;

    public void Notify(GunConfig gunConfig) => _onEventInvoked?.Invoke(gunConfig);

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