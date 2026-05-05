using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject-based event system that allows for decoupled communication between game systems.
/// Objects can subscribe to this event via <see cref="ScriptableEventListener"/> and be notified when <see cref="Invoke"/> is called.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Events/New Scriptable Event")]
public class ScriptableEvent : ScriptableObject
{
    private List<ScriptableEventListener> _listeners = new();

    /// <summary>
    /// Iterates through all registered <see cref="ScriptableEventListener"/> instances and triggers their notification logic.
    /// </summary>
    public void Invoke()
    {
        foreach (var listener in _listeners)
            listener.Notify();
    }

    /// <summary>
    /// Registers a <see cref="ScriptableEventListener"/> to the internal notification list.
    /// </summary>
    /// <param name="listener">The listener component to add.</param>
    public void AddListener(ScriptableEventListener listener) => _listeners.Add(listener);

    /// <summary>
    /// Removes a <see cref="ScriptableEventListener"/> from the internal notification list.
    /// </summary>
    /// <param name="listener">The listener component to remove.</param>
    public void RemoveListener(ScriptableEventListener listener) => _listeners.Remove(listener);
}