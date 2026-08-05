using System.Collections.Generic;
using UnityEngine;

namespace ToolkitByJonathan
{
    [CreateAssetMenu(menuName = "Scriptable Events/New Scriptable Event")]
    public class ScriptableEvent : ScriptableObject
    {
        private List<ScriptableEventListener> _listeners = new();

        public void Invoke()
        {
            for (int i = _listeners.Count; i >= 0; i--)
            {
                if (i < _listeners.Count)
                    _listeners[i]?.Notify();
            }
        }

        public void AddListener(ScriptableEventListener listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener listener) => _listeners.Remove(listener);
    }
}