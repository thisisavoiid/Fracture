using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A listener component that subscribes to a <see cref="ScriptableEvent"/> and triggers a <see cref="UnityEvent"/> 
/// response when notified. This facilitates decoupled communication between ScriptableObject architecture 
/// and scene-based GameObjects.
/// </summary>
public class ScriptableEventListener : MonoBehaviour
{
    [Header("Event Configuration")]
    [Tooltip("The ScriptableEvent asset this component listens to.")]
    [SerializeField] private ScriptableEvent _event;

    [Header("Responses")]
    [Tooltip("The UnityEvent that is invoked when the ScriptableEvent is triggered.")]
    [SerializeField] private UnityEvent _onEventInvoked;

    /// <summary>
    /// Triggered by the <see cref="ScriptableEvent"/> to execute the associated <see cref="UnityEvent"/>.
    /// </summary>
    public void Notify() => _onEventInvoked?.Invoke();

    /// <summary>
    /// Registers this listener to the <see cref="ScriptableEvent"/> when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (_event == null)
            return;

        _event.AddListener(this);
    }

    /// <summary>
    /// Unregisters this listener from the <see cref="ScriptableEvent"/> when the component is disabled 
    /// to prevent null references.
    /// </summary>
    private void OnDisable()
    {
        if (_event == null)
            return;

        _event.RemoveListener(this);
    }
}