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
    public Transform TargetTransform => _targetTransform;
    [SerializeField] private LayerMask _targetMask;
    public LayerMask TargetMask => _targetMask;
    [SerializeField] private float _viewDistance;
    public float ViewDistance => _viewDistance;
    [SerializeField] private float _calmDownDistance;
    public float CalmDownDistance => _calmDownDistance;
    [SerializeField] private float _minAttackDistance;
    public float MinAttackDistance => _minAttackDistance;

    // References to Required Components
    private NavMeshAgent _navmeshAgent;
    public NavMeshAgent Agent => _navmeshAgent;

    private OverlapSphereDetector _overlapSphereDetector;
    public OverlapSphereDetector OverlapSphereDetector => _overlapSphereDetector;

    private RayCastDetector _raycastDetector;
    public RayCastDetector RaycastDetector => _raycastDetector;

    private InventoryController _inventoryController;
    public InventoryController InventoryController => _inventoryController;

    private ItemFactory _itemFactory;
    public ItemFactory ItemFactory => _itemFactory;

    private ItemSlotController _itemSlotController;
    public ItemSlotController ItemSlotController => _itemSlotController;
    [SerializeField] private Transform _headTransform;
    public Transform HeadTransform => _headTransform;
    private Transform _transform;
    public Transform Transform => _transform;
    #endregion

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

        SetState(new PatrolState());
    }

    private void Update()
    {
        if (_currentState == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Couldn't execute current state on GameObject '{gameObject.name}' because the current state is null.");
            return;
        }

        _currentState.Run(gameObject);
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.Exit(gameObject);

        _currentState = state;

        if (_currentState != null)
            _currentState.Enter(gameObject);
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