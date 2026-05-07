using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Events/New Scriptable Event (GunConfig)")]
public class GunConfigEvent : ScriptableObject
{
    private List<GunConfigEventListener> _listeners = new();

    public void Invoke(GunConfig gunConfig)
    {
        foreach (var listener in _listeners)
            listener.Notify(gunConfig);
    }

    public void AddListener(GunConfigEventListener listener) => _listeners.Add(listener);

    public void RemoveListener(GunConfigEventListener listener) => _listeners.Remove(listener);
}