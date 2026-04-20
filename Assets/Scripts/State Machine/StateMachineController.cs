using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineController : MonoBehaviour, IStateMachine
{
    [SerializeField] private State _defaultState;
    [SerializeField] private List<StateTransition> _stateTransitions;

    private State _currentState;
    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent ??= GetComponent<NavMeshAgent>();
    [SerializeField] private Transform _targetTransform;
    public Transform TargetTransform => _targetTransform;

    private void Awake()
    {
        SetState(_defaultState);
    }

    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null -");
            return;
        }

        _currentState.Run(this);

        if (!_stateTransitions.Select(entry => entry.State).Contains(_currentState))
            return;

        StateTransition stateEntry = _stateTransitions.First(
            entry => entry.State == _currentState
        );

        foreach (var stateTransition in stateEntry.TargetStateConditions)
        {
            if (stateTransition.State == _currentState)
            {
                Debug.LogWarning($"[STATE MACHINE CONTROLLER] Can't transition to state {stateTransition.State} as it's currently active. Please remove its entry -");
                continue;
            }

            if (stateTransition.Condition == null)
            {
                Debug.LogWarning($"[STATE MACHINE CONTROLLER] {stateTransition.State} has no transition condition attached -");
                continue;
            }

            if (stateTransition.Modifier == ConditionModifier.IsEqual)
            {
                if (stateTransition.Condition.IsMet())
                {
                    SetState(stateTransition.State);
                    return;
                }
            }

            if (stateTransition.Modifier == ConditionModifier.IsNotEqual)
            {
                if (!stateTransition.Condition.IsMet())
                {
                    SetState(stateTransition.State);
                    return;
                }
            }
        }
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.Exit(this);

        _currentState = state;

        if (_currentState != null)
            _currentState.Enter(this);
    }
}