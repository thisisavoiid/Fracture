using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(InventoryController))]
[RequireComponent(typeof(ItemFactory))]
[RequireComponent(typeof(ItemSlotController))]
public class EnemyBrain : MonoBehaviour
{
    #region FSM Variables
    private State _currentState;
    #endregion

    #region Dependencies
    [Header("Detection Settings")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _viewDistance;
    [SerializeField] private float _calmDownDistance;
    [SerializeField] private float _minAttackDistance;

    private NavMeshAgent _navmeshAgent;
    private OverlapSphereDetector _overlapSphereDetector;
    private RayCastDetector _raycastDetector;
    private InventoryController _inventoryController;
    private ItemFactory _itemFactory;
    private ItemSlotController _itemSlotController;
    [SerializeField] private Transform _headTransform;
    private Transform _transform;
    #endregion

    private Dictionary<State, List<Transition>> _states = new();
    private float _distanceToPlayer;

    private void Awake()
    {
        _transform = transform;

        _navmeshAgent = GetComponent<NavMeshAgent>();
        _overlapSphereDetector = GetComponent<OverlapSphereDetector>();
        _raycastDetector = GetComponent<RayCastDetector>();
        _inventoryController = GetComponent<InventoryController>();
        _itemFactory = GetComponent<ItemFactory>();
        _itemSlotController = GetComponent<ItemSlotController>();

        _overlapSphereDetector.SetRadius(_viewDistance / 2);

        State patrolState = new PatrolState(_navmeshAgent);
        State chaseState = new ChaseState(_navmeshAgent, _targetTransform);
        State attackState = new AttackState(_itemSlotController, _headTransform, _targetTransform);

        _states.Add(
            patrolState, new()
            {
                new Transition(chaseState, () => CanSeePlayer())
            }
        );

        _states.Add(
            chaseState, new()
            {
                new Transition(attackState, () => _distanceToPlayer <= _minAttackDistance),
                new Transition(patrolState, () => _distanceToPlayer >= _calmDownDistance)
            }
        );

        _states.Add(
           attackState, new()
           {
                new Transition(chaseState, () => _distanceToPlayer > _minAttackDistance),
           }
       );

        SetState(patrolState);
    }

    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null.");
            return;
        }

        _currentState.Run();

        _distanceToPlayer = (_targetTransform.position - _transform.position).magnitude;

        foreach (Transition transition in _states[_currentState])
        {
            if (transition.Condition() == true)
                SetState(transition.TargetState);
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
}