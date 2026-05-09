using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Events/New Scriptable Event (GunContext)")]
public class GunContextEvent : ScriptableObject
{
    private List<GunContextEventListener> _listeners = new();

    public void Invoke(GunContext gunContext)
    {
        foreach (var listener in _listeners)
            listener.Notify(gunContext);
    }

    public void AddListener(GunContextEventListener listener) => _listeners.Add(listener);

    public void RemoveListener(GunContextEventListener listener) => _listeners.Remove(listener);
}