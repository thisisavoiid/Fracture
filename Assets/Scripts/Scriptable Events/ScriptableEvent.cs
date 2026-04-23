using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Events/New Scriptable Event")]
public class ScriptableEvent : ScriptableObject
{
    private List<ScriptableEventListener> _listeners = new();

    public void Invoke()
    {
        foreach (var listener in _listeners)
            listener.Notify();
    }

    public void AddListener(ScriptableEventListener listener) => _listeners.Add(listener);
    public void RemoveListener(ScriptableEventListener listener) => _listeners.Remove(listener);
}