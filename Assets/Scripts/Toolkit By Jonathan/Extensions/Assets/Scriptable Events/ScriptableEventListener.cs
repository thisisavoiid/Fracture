using UnityEngine;
using UnityEngine.Events;

namespace ToolkitByJonathan
{
    public class ScriptableEventListener : MonoBehaviour
    {
        [Header("Event Configuration")]
        [Tooltip("The ScriptableEvent asset this component listens to.")]
        [SerializeField] private ScriptableEvent _event;

        [Header("Responses")]
        [Tooltip("The UnityEvent that is invoked when the ScriptableEvent is triggered.")]
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
}