using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(InventoryController))]
[RequireComponent(typeof(ItemFactory))]
[RequireComponent(typeof(ItemSlotController))]
[RequireComponent(typeof(Battery))]
[RequireComponent(typeof(Timer))]

public class RobotBrain : MonoBehaviour
{
    #region FSM Variables
    private State _currentState;
    #endregion

    #region Dependencies
    [Header("Detection Settings")]
    [Tooltip("Reference to the TransformVariable of the target (e.g., the player).")]
    [SerializeField] private TransformVariable _targetTransform;

    [Tooltip("LayerMask used to filter target objects during detection.")]
    [SerializeField] private LayerMask _targetMask;

    [Tooltip("Maximum distance the robot can see.")]
    [SerializeField] private float _viewDistance;

    [Tooltip("Distance at which the robot stops chasing and returns to patrol.")]
    [SerializeField] private float _calmDownDistance;

    [Tooltip("Minimum distance required to initiate an attack.")]
    [SerializeField] private float _minAttackDistance;

    [Tooltip("Transform of the head used for line-of-sight checks and attack aiming.")]
    [SerializeField] private Transform _headTransform;

    [Header("State Settings")]
    [Header("Patrol State")]
    [Tooltip("Movement speed of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolSpeed;

    [Tooltip("Acceleration of the NavMeshAgent during patrol.")]
    [SerializeField] private float _patrolAcceleration;

    [Tooltip("List of waypoints for the robot to follow in sequence.")]
    [SerializeField] private List<Transform> _patrolWaypoints;
    
    [Header("Chase State")]
    [Tooltip("Movement speed of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseSpeed;

    [Tooltip("Acceleration of the NavMeshAgent during pursuit.")]
    [SerializeField] private float _chaseAcceleration;

    [Header("Go To Charge Station State")]
    [Tooltip("Transform reference for the charging station location.")]
    [SerializeField] private Transform _chargeStationTransform;

    [Tooltip("Movement speed when heading to the charging station.")]
    [SerializeField] private float _goToChargeStationSpeed;

    [Tooltip("Acceleration when heading to the charging station.")]
    [SerializeField] private float _goToChargeAcceleration;

    [Header("Reload Settings")]
    [Tooltip("Time in seconds between reload and another attack. (Only applied if the currently active item is Weapon child!)")]
    [SerializeField] private float _reloadTime;


    private Battery _battery;
    private NavMeshAgent _navmeshAgent;
    private OverlapSphereDetector _overlapSphereDetector;
    private RayCastDetector _raycastDetector;
    private ItemSlotController _itemSlotController;
    private Transform _transform;
    private Timer _reloadTimer;
    #endregion

    private Dictionary<State, List<Transition>> _states = new();

    private void Start()
    {
        _transform = transform;

        _navmeshAgent = GetComponent<NavMeshAgent>();
        _overlapSphereDetector = GetComponent<OverlapSphereDetector>();
        _raycastDetector = GetComponent<RayCastDetector>();
        _itemSlotController = GetComponent<ItemSlotController>();
        _battery = GetComponent<Battery>();
        _reloadTimer = GetComponent<Timer>();

        _reloadTimer.SetTime(new TimeMS() );
        _reloadTimer.Start();

        _overlapSphereDetector.SetRadius(_viewDistance / 2);

        State patrolState = new PatrolState(
            _navmeshAgent,
            _patrolWaypoints,
            _patrolSpeed,
            _patrolAcceleration,
            _battery
        );

        State chaseState = new ChaseState(
            _navmeshAgent,
            _targetTransform.Value,
            _chaseSpeed,
            _chaseAcceleration,
            _battery
        );

        State attackState = new AttackState(
            _itemSlotController,
            _headTransform,
            _targetTransform.Value,
            _reloadTimer,
            _battery
        );

        State goToChargeStationState = new GoToChargeStationState(
            _chargeStationTransform,
            _navmeshAgent,
            _goToChargeStationSpeed,
            _goToChargeAcceleration
        );

        State chargeBatteryState = new ChargeBatteryState(_battery);

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
                new Transition(chargeBatteryState, () => (_chargeStationTransform.position -_transform.position).magnitude <= _navmeshAgent.stoppingDistance)
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
                new Transition(patrolState, () => GetDistanceToTarget() >= _calmDownDistance),
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

        SetState(patrolState);
    }

    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null -");
            return;
        }

        _currentState.Run();

        if (_states.ContainsKey(_currentState))
        {
            foreach (Transition transition in _states[_currentState])
            {
                if (transition.Condition() == true)
                    SetState(transition.TargetState);
            }
        }
        else
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] State {_currentState.GetType().Name} has no transitions -");
            return;
        }
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

    private float GetDistanceToTarget()
    {
        if (_targetTransform == null)
            return 0.0f;

        return (_targetTransform.Value.position - transform.position).magnitude;
    }
}