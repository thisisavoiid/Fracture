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
[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(Battery))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(InventoryController))]
public class RobotBrain : MonoBehaviour, ICollectionMember
{
    #region FSM Variables
    private State _currentState;
    #endregion

    #region Dependencies
    [BoxGroup("Detection Settings")]
    [Tooltip("Reference to the TransformVariable of the target (e.g., the player).")]
    [SerializeField] private TransformVariable _targetTransform;

    [BoxGroup("Detection Settings")]
    [Tooltip("LayerMask used to filter target objects during detection.")]
    [SerializeField] private LayerMask _targetMask;

    [BoxGroup("Detection Settings")]
    [Tooltip("Maximum distance the robot can see.")]
    [SerializeField] private float _viewDistance;

    [BoxGroup("Detection Settings")]
    [Tooltip("Distance at which the robot stops chasing and returns to patrol.")]
    [SerializeField] private float _calmDownDistance;

    [BoxGroup("Detection Settings")]
    [Tooltip("Minimum distance required to initiate an attack.")]
    [SerializeField] private float _minAttackDistance;

    [BoxGroup("Detection Settings")]
    [Tooltip("Transform of the head used for line-of-sight checks and attack aiming.")]
    [SerializeField] private Transform _headTransform;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Movement speed of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolSpeed;

    [BoxGroup("Patrol Settings")]
    [Tooltip("Acceleration of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolAcceleration;

    [BoxGroup("Patrol Settings")]
    [Tooltip("List of waypoints for the robot to follow in sequence.")]
    [SerializeField] private List<Transform> _patrolWaypoints;

    [BoxGroup("Chase Settings")]
    [Tooltip("Movement speed of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseSpeed;

    [BoxGroup("Chase Settings")]
    [Tooltip("Acceleration of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseAcceleration;

    [BoxGroup("Charging Settings")]
    [Tooltip("Transform reference for the charging station location.")]
    [SerializeField] private Transform _chargeStationTransform;

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
    [Tooltip("The time required for the robot to turn to the target when being in attack state.")]
    [SerializeField] private float _turnToTargetSpeed = 7.5f;

    [BoxGroup("Combat & Timers")]
    [SerializeField] private TimeMS _searchDuration;

    [BoxGroup("Combat & Timers")]
    [SerializeField] private Timer _searchTimer;

    [BoxGroup("Combat & Timers")]
    [SerializeField] private Timer _reloadTimer;

    [BoxGroup("Combat & Timers")]
    [SerializeField] private Animator _animator;

    [BoxGroup("Initialization Events")]
    [SerializeField] private UnityEvent OnRobotInitialize;

    private Battery _battery;
    private NavMeshAgent _agent;
    private OverlapSphereDetector _overlapSphereDetector;
    private RayCastDetector _raycastDetector;
    private InventoryController _inventory;
    private Transform _transform;
    #endregion

    private Dictionary<State, List<Transition>> _states = new();

    /// <summary>
    /// Initializes dependencies, configures the state machine transitions, and sets the initial <see cref="PatrolState"/>.
    /// </summary>
    private void Start()
    {
        _transform = transform;

        _agent = GetComponent<NavMeshAgent>();
        _overlapSphereDetector = GetComponent<OverlapSphereDetector>();
        _raycastDetector = GetComponent<RayCastDetector>();
        _inventory = GetComponent<InventoryController>();
        _battery = GetComponent<Battery>();

        _overlapSphereDetector.SetRadius(_viewDistance / 2);

        ConfigureStateMachine();

        Subscribe();

        OnRobotInitialize?.Invoke();
    }

    private void ConfigureStateMachine()
    {
        State patrolState = new PatrolState(
            _agent,
            _patrolWaypoints,
            _patrolSpeed,
            _patrolAcceleration,
            _battery
        );

        State chaseState = new ChaseState(
            _agent,
            _targetTransform.Value,
            _chaseSpeed,
            _chaseAcceleration,
            _battery
        );

        State attackState = new AttackState(
            _inventory,
            _headTransform,
            _targetTransform.Value,
            _reloadDuration,
            _reloadTimer,
            _battery,
            _turnToTargetSpeed
        );

        State goToChargeStationState = new GoToChargeStationState(
            _chargeStationTransform,
            _agent,
            _goToChargeStationSpeed,
            _goToChargeAcceleration
        );

        State chargeBatteryState = new ChargeBatteryState(_battery);

        State searchState = new SearchState(
            _targetTransform,
            _agent,
            _searchTimer,
            _searchDuration,
            _animator
        );

        _states.Add(
            patrolState, new()
            {
                new Transition(goToChargeStationState, () => _battery.IsDrained),
                new Transition(chaseState, () => CanSeePlayer())
            }
        );

        _states.Add(
            goToChargeStationState, new()
            {
                new Transition(chargeBatteryState, () => _agent.remainingDistance <= 0.25f)
            }
        );

        _states.Add(
            chargeBatteryState, new()
            {
                new Transition(patrolState, () => _battery.IsCharged)
            }
        );

        _states.Add(
            chaseState, new()
            {
                new Transition(attackState, () => GetDistanceToTarget() <= _minAttackDistance),
                new Transition(searchState, () => CanSeePlayer() == false),
                new Transition(goToChargeStationState, () => _battery.IsDrained)
            }
        );

        _states.Add(
           attackState, new()
           {
                new Transition(chaseState, () => GetDistanceToTarget() > _minAttackDistance),
                new Transition(goToChargeStationState, () => _battery.IsDrained),
           }
       );

        _states.Add(
            searchState, new()
            {
                new Transition(patrolState, () => IsSearchTimeOver()),
                new Transition(chaseState, () => CanSeePlayer())
            }
        );

        SetState(patrolState);
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

        _currentState.Run();
        SetAnimatorValues();

        if (_states.ContainsKey(_currentState))
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
        else
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] State {_currentState.GetType().Name} has no transitions -");
            return;
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

        bool isInAttackState = _currentState.GetType() == typeof(AttackState);

        _animator.SetBool("IsAttacking", isInAttackState);
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }
    /// <summary>
    /// Handles the transition logic by exiting the current <see cref="State"/> and entering the new target <see cref="State"/>.
    /// </summary>
    /// <param name="state">The new state to transition into.</param>
    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.Exit();

        _currentState = state;

        if (_currentState != null)
            _currentState.Enter();
    }

    /// <summary>
    /// Performs a combined check using <see cref="OverlapSphereDetector"/> and <see cref="RayCastDetector"/> 
    /// to determine if the target is within sight and range.
    /// </summary>
    /// <returns>True if the player is detected and visible; otherwise, false.</returns>
    public bool CanSeePlayer()
    {
        List<Collider> foundColliders = _overlapSphereDetector.GetColliders(_targetMask);
        Collider closestTargetCollider = GetClosestCollider(_transform.position, foundColliders);

        if (closestTargetCollider == null)
            return false;

        Vector3 dir = closestTargetCollider.transform.position - _transform.position;

        bool isPlayerInSight = _raycastDetector.Check(_transform.position, dir, out RaycastHit hit, _viewDistance);

        if (!isPlayerInSight)
            return false;

        return hit.collider.gameObject == closestTargetCollider.gameObject;
    }

    public bool IsSearchTimeOver() => _searchTimer.GetRemainingTime().TotalSeconds <= 0.0f;

    /// <summary>
    /// Filters a list of colliders to find the one closest to a specific origin point.
    /// </summary>
    /// <param name="origin">The starting point for the distance calculation.</param>
    /// <param name="colliders">The list of colliders to evaluate.</param>
    /// <returns>The closest <see cref="Collider"/> or null if the list is empty.</returns>
    private Collider GetClosestCollider(Vector3 origin, List<Collider> colliders)
    {
        if (colliders == null || colliders.Count == 0)
            return null;

        if (colliders.Count == 1)
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
        if (_patrolWaypoints.Count == 0 || _patrolWaypoints == null)
            return;

        Gizmos.color = Color.blue;

        Gizmos.DrawLineStrip(_patrolWaypoints.Select(waypoint => waypoint.position).ToArray(), true);

        if (_chargeStationTransform == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(_chargeStationTransform.position, Vector3.one);
    }

    /// <summary>
    /// Calculates the distance between the robot and the target transform.
    /// </summary>
    /// <returns>The magnitude of the distance as a <see cref="float"/>.</returns>
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
}