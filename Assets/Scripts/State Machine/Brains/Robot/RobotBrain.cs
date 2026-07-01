using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// The central processing unit for the robot AI, implementing a Finite State Machine. 
/// It manages transitions between states like Patrol, Chase, Attack, and Charging based on 
/// environmental triggers, battery levels, and player visibility.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(Battery))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(InventoryController))]
[RequireComponent(typeof(NavMeshPointGenerator))]
public class RobotBrain : MonoBehaviour, ICollectionMember
{
    #region FSM Variables
    [BoxGroup("State Machine")]
    [ReadOnly][SerializeField] private State _currentState;
    #endregion

    #region Dependencies
    [BoxGroup("Detection Settings")]
    [Tooltip("Reference to the TransformVariable of the target (e.g., the player).")]
    [SerializeField] private TransformVariable _targetTransform;

    [BoxGroup("Detection Settings")]
    [Tooltip("LayerMask used to filter target objects during detection.")]
    [SerializeField] private LayerMask _targetLayers;

    [BoxGroup("Detection Settings")]
    [SerializeField] private LayerMask _robotLayer;

    [BoxGroup("Detection Settings")]
    [Tooltip("Maximum distance the robot can see.")]
    [SerializeField] private float _viewDistance;

    [BoxGroup("Detection Settings")]
    [Tooltip("Distance at which the robot stops chasing and returns to patrol.")]
    [SerializeField] private float _calmDownDistance;

    [BoxGroup("Detection Settings")]
    [Tooltip("Minimum distance required to initiate an attack.")]
    [SerializeField] private float _minAttackDistance;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Movement speed of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolSpeed;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Acceleration of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolAcceleration;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Amount of waypoints to be generated per robot instance.")]
    [SerializeField] private int _waypointCount = 5;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Radius around the robot instances which the waypoints are going to be placed in.")]
    [SerializeField] private float _patrolRadius = 30;

    [BoxGroup("Flee Settings")]
    [SerializeField] private float _fleeDistance = 15;

    [BoxGroup("Flee Settings")]
    [SerializeField] private float _fleeTriggerRadius = 5;

    [BoxGroup("Flee Settings")]
    [SerializeField] private float _fleeFallbackRadius = 30;

    [BoxGroup("Flee Settings")]
    [SerializeField] private float _fleePositionReachedThreshold = 1.25f;

    [BoxGroup("Flee Settings")]
    [SerializeField] private int _minNeighbourCount = 1;

    [BoxGroup("Chase Settings")]
    [Tooltip("Movement speed of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseSpeed;

    [BoxGroup("Chase Settings")]
    [Tooltip("Acceleration of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseAcceleration;

    [BoxGroup("Charging Settings")]
    [Tooltip("Movement speed when heading to the charging station.")]
    [SerializeField] private float _goToChargeStationSpeed;

    [BoxGroup("Charging Settings")]
    [Tooltip("Acceleration when heading to the charging station.")]
    [SerializeField] private float _goToChargeAcceleration;

    [BoxGroup("Combat & Timers")]
    [Tooltip("Time in seconds between reload and another attack. (Only applied if the currently active item is Weapon child!)")]
    [SerializeField] private TimeMS _reloadDuration;

    [BoxGroup("Combat & Timers")]
    [SerializeField] private TimeMS _searchDuration;

    [SerializeField] private float _searchPositionReachedTreshold = 1.25f;

    [BoxGroup("Combat & Timers")]
    [Tooltip("The time required for the robot to turn to the target when being in attack state.")]
    [SerializeField] private float _turnSpeed = 7.5f;

    [BoxGroup("Animator")]
    [SerializeField] private Animator _animator;

    [BoxGroup("Initialization Events")]
    [SerializeField] private UnityEvent OnRobotInitialize;

    [BoxGroup("Patrol Runtime Data")]
    [ReadOnly][SerializeField] private List<Vector3> _patrolWaypoints = new();

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private Battery _battery;

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private NavMeshAgent _agent;

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private RayCastDetector _raycastDetector;

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private InventoryController _inventory;

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private Transform _transform;

    [BoxGroup("Internal Components")]
    [Required][SerializeField] private NavMeshPointGenerator _pointGenerator;

    [BoxGroup("Charging Runtime Data")]
    [ReadOnly][SerializeField] private Vector3 _chargeStationPosition;

    [BoxGroup("Debugging")]
    [SerializeField] private bool _enableDebugMode = false;

    [SerializeField] private float _robotScanRadius = 20.0f;

    #endregion

    private Dictionary<State, List<Transition>> _states = new();

    private State chaseState;
    private State chargeBatteryState;
    private State goToChargeStationState;
    private State patrolState;
    private State goToClosestRobotState;
    private State fleeState;
    private State attackState;
    private State searchState;

    private void Awake()
    {
        GeneratePatrolWaypoints();
        GenerateChargeStationPoint();
        InitializeStates();
        InitializeTransitions();
        Subscribe();
        OnRobotInitialize?.Invoke();
    }

    private void Start()
    {
        SetInitialState();
    }

    private void SetInitialState() => SetState(patrolState);

    private void InitializeTransitions()
    {
        _states.Add(
            patrolState, new()
            {
                new Transition(goToChargeStationState, () => _battery.IsDrained),
                new Transition(chaseState, () => CanSeePlayer())
            }
        );

        _states.Add(
            chaseState, new()
            {
                new Transition(
                    goToChargeStationState,
                    () => _battery.IsDrained
                ),
                new Transition(
                    attackState,
                    () => GetDistanceToTarget() <= _minAttackDistance && CanSeePlayer()
                ),
                new Transition(
                    searchState,
                    () => !CanSeePlayer()
                )
            }
        );

        _states.Add(
            goToChargeStationState, new()
            {
                new Transition(chargeBatteryState, () => !_agent.pathPending && _agent.remainingDistance <= 0.25f)
            }
        );

        _states.Add(
            chargeBatteryState, new()
            {
                new Transition(patrolState, () => _battery.IsCharged)
            }
        );

        _states.Add(
            goToClosestRobotState, new()
            {
                new Transition(
                    attackState,
                    () => (goToClosestRobotState as RobotGoToClosestRobotState).ClosestTargetReached
                )
            }
        );

        _states.Add(
            fleeState, new()
            {
                new Transition(
                    attackState,
                    () => CanSeePlayer() && (fleeState as RobotFleeState).IsFleePositionReached
                ),
                new Transition(
                    patrolState,
                    () => !CanSeePlayer() && (fleeState as RobotFleeState).IsFleePositionReached
                )
            }
        );

        _states.Add(
            attackState, new()
            {
                new Transition(
                    goToChargeStationState,
                    () => _battery.IsDrained
                ),
                new Transition(
                    searchState,
                    () => !CanSeePlayer()
                ),
                new Transition(
                    chaseState,
                    () => GetDistanceToTarget() > _minAttackDistance &&
                    CanSeePlayer()
                ),
                new Transition(
                    goToClosestRobotState,
                    () => GetSurroundingRobotColliders().Any() &&
                    GetDistanceToTarget() <= _fleeTriggerRadius && 
                    !(goToClosestRobotState as RobotGoToClosestRobotState).ClosestTargetReached
                ),
                new Transition(
                    fleeState,
                    () => !GetSurroundingRobotColliders().Any() &&
                    GetDistanceToTarget() <= _fleeTriggerRadius
                )
            }
        );

        _states.Add(
            searchState, new()
            {
                new Transition(
                    chaseState,
                    () => CanSeePlayer()
                ),
                new Transition(
                    patrolState,
                    () => !CanSeePlayer() && (searchState as RobotSearchState).IsSearchTimeOver
                )
            }
        );
    }

    private void InitializeStates()
    {
        patrolState = new RobotPatrolState(
            _agent,
            _patrolWaypoints,
            _patrolSpeed,
            _patrolAcceleration,
            _battery
        );

        chaseState = new RobotChaseState(
            _agent,
            _targetTransform,
            _chaseSpeed,
            _chaseAcceleration,
            _battery
        );

        goToChargeStationState = new RobotGoToChargeStationState(
            _chargeStationPosition,
            _agent,
            _goToChargeStationSpeed,
            _goToChargeAcceleration
        );

        goToClosestRobotState = new RobotGoToClosestRobotState(
            _agent,
            this
        );

        chargeBatteryState = new RobotChargeBatteryState(_battery);

        fleeState = new RobotFleeState(
            _pointGenerator,
            _agent,
            _targetTransform,
            _fleeFallbackRadius,
            _fleeDistance,
            _fleePositionReachedThreshold
        );

        attackState = new RobotAttackState(
            _agent,
            _inventory,
            _targetTransform,
            _turnSpeed
        );

        searchState = new RobotSearchState(
            _targetTransform,
            _searchDuration,
            _agent,
            _animator,
            _searchPositionReachedTreshold
        );
    }

    private void GeneratePatrolWaypoints()
    {
        for (int i = 0; i < _waypointCount; i++)
        {
            Vector3 waypoint = _pointGenerator.FindPosition(_patrolRadius);
            _patrolWaypoints.Add(waypoint);
        }
    }

    private void GenerateChargeStationPoint()
    {
        Vector3 point = _pointGenerator.FindPosition(_patrolRadius);
        _chargeStationPosition = point;
    }

    /// <summary>
    /// Executes the current state's logic and evaluates transitions to determine if a state switch is required.
    /// </summary>
    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null -");
            return;
        }

        _currentState.Run(Time.deltaTime);

        SetAnimatorValues();

        if (_states.TryGetValue(_currentState, out List<Transition> transitions))
        {
            foreach (Transition transition in _states[_currentState])
            {
                if (transition.Condition() == true)
                {
                    SetState(transition.TargetState);
                    break;
                }
            }
        }
    }

    private void SetAnimatorValues()
    {
        if (_animator == null)
            return;

        if (_agent == null)
            return;

        if (_currentState == null)
            return;

        if ((attackState as RobotAttackState).IsFiringAShot)
            _animator.SetTrigger("ShotFired");

        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.Exit();

        _currentState = state;

        if (_currentState != null)
            _currentState.Enter();
    }

    public bool CanSeePlayer()
    {
        Collider[] foundColliders = Physics.OverlapSphere(
            _agent.transform.position,
            _viewDistance / 2,
            _targetLayers
        );

        Collider closestTargetCollider = GetClosestCollider(_transform.position, foundColliders);

        if (closestTargetCollider == null)
            return false;

        Vector3 dir = closestTargetCollider.transform.position - _transform.position;

        bool isPlayerInSight = _raycastDetector.Check(_transform.position, dir, out RaycastHit hit, _viewDistance);

        if (!isPlayerInSight)
            return false;

        return hit.collider.gameObject == closestTargetCollider.gameObject;
    }

    private Collider GetClosestCollider(Vector3 origin, Collider[] colliders)
    {
        if (colliders == null || colliders.Count() == 0)
            return null;

        if (colliders.Count() == 1)
            return colliders[0];

        return colliders
            .OrderBy(c => (c.transform.position - origin).sqrMagnitude)
            .FirstOrDefault();
    }

    /// <summary>
    /// Visualizes patrol paths and the charging station position within the Unity Editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.magenta;
        labelStyle.fontStyle = FontStyle.Bold;

        if (_patrolWaypoints.Count != 0 && _patrolWaypoints != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLineStrip(_patrolWaypoints.Select(waypoint => waypoint).ToArray(), true);
        }

        if (_chargeStationPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_chargeStationPosition, Vector3.one);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * _viewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _robotScanRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _minAttackDistance);
        Gizmos.DrawRay(transform.position, transform.forward * _minAttackDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _fleeTriggerRadius);
    }

    public Collider[] GetSurroundingRobotColliders()
    {
        Collider[] robotColliders = Physics.OverlapSphere(
            _agent.transform.position,
            _robotScanRadius,
            _robotLayer
        )
        .Where(
            obj => obj.transform.root.gameObject != _agent.transform.root.gameObject
        )
        .ToArray();

        return robotColliders;
    }

    private float GetDistanceToTarget()
    {
        if (_targetTransform == null)
            return 0.0f;

        return (_targetTransform.Value.position - transform.position).magnitude;
    }

    private void OnEnable()
    {
        Subscribe();
    }
    // private void OnDisable()
    // {
    //     Unsubscribe();
    // }

    public void Subscribe()
    {
        EnemyCollectionManager.Instance?.Subscribe(this);
    }

    public void Unsubscribe()
    {
        EnemyCollectionManager.Instance?.Unsubscribe(this);
    }

    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForcePatrolState() => SetState(patrolState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceChaseState() => SetState(chaseState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceGoToChargeStationState() => SetState(goToChargeStationState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceChargeBatteryState() => SetState(chargeBatteryState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceGoToClosestRobotState() => SetState(goToClosestRobotState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceFleeState() => SetState(fleeState);
    [Button]
    [ShowIf("_enableDebugMode")]
    public void ForceAttackState() => SetState(attackState);
}