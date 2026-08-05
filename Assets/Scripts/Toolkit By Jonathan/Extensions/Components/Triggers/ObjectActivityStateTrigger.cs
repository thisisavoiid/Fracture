using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToolkitByJonathan
{
    public class ObjectActivityStateTrigger : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [Tooltip("Determines whether this GameObject starts active when the scene begins.")]
        [SerializeField]
        private bool _isActiveByDefault = false;

        [BoxGroup("Settings")]
        [Tooltip("Determines whether this GameObject is activated or deactivated when the trigger is called.")]
        [SerializeField]
        private bool _setActiveWhenTriggered = true;

        [BoxGroup("Trigger Target")]
        [Tooltip("The GameObject that is affected by this trigger. A trigger call will be ignored if no target is assigned.")]
        [SerializeField]
        [InfoBox("This field determines which game object is supposed to be influenced by a trigger.")]
        [Required]
        private GameObject _triggerTarget;

        [BoxGroup("Timer")]
        [Tooltip("The duration for which the GameObject remains in the triggered state before being reverted.")]
        [SerializeField]
        private TimeMS _timeEnabledAfterTriggered = new();

        [BoxGroup("Runtime")]
        [Tooltip("The amount of time that has passed since the trigger was activated.")]
        [SerializeField]
        [ReadOnly]
        private float _timePassedAfterTrigger = 0.0f;

        [BoxGroup("Runtime")]
        [Tooltip("Indicates whether this object is currently affected by an active trigger cycle.")]
        [SerializeField]
        [ReadOnly]
        private bool _isCurrentlyTriggered = false;

        [BoxGroup("Runtime")]
        [Tooltip("Indicates whether the trigger timer coroutine is currently running.")]
        [SerializeField]
        [ReadOnly]
        private bool _isTimerCycleAlreadyRunning = false;

        private void Awake()
        {
            if (_triggerTarget.activeInHierarchy != _isActiveByDefault)
                _triggerTarget.SetActive(_isActiveByDefault);

            ResetTriggerTime();
        }

        private void ResetTriggerTime()
        {
            _timePassedAfterTrigger = 0.0f;
        }

        [Button("Force Trigger")]
        public void Trigger()
        {
            Debug.Log("Triggered!");
            if (_triggerTarget == null)
                return;

            ResetTriggerTime();

            if (_triggerTarget.activeInHierarchy != _setActiveWhenTriggered)
                _triggerTarget.SetActive(_setActiveWhenTriggered);

            if (_isTimerCycleAlreadyRunning)
            {
                StopCoroutine(TriggerCycle());
                _isTimerCycleAlreadyRunning = false;
            }

            StartCoroutine(TriggerCycle());
        }

        private IEnumerator TriggerCycle()
        {
            _isTimerCycleAlreadyRunning = true;

            float targetTime = _timeEnabledAfterTriggered.TotalSeconds;

            while (_timePassedAfterTrigger <= targetTime)
            {
                _timePassedAfterTrigger += Time.deltaTime;
                yield return null;
            }

            if (_triggerTarget.activeInHierarchy == _setActiveWhenTriggered)
                _triggerTarget.SetActive(!_setActiveWhenTriggered);

            _isTimerCycleAlreadyRunning = false;

            yield break;
        }
    }
}