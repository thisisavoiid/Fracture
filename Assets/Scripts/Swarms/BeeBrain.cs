using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(Rigidbody))]
public class BeeBrain : Swarm
{
    [Header("Debugging")]
    [Tooltip("The designated leader of this specific swarm group.")]
    [SerializeField] private Swarm _leaderSwarm;

    [Header("Movement Settings")]
    [Tooltip("Movement speed for regular swarm members.")]
    [SerializeField] private float _swarmSpeed;

    [Tooltip("Movement speed for the leader bee.")]
    [SerializeField] private float _leaderSpeed;

    [Tooltip("Vertical offset applied to the leader's position for followers to target.")]
    [SerializeField] private float _yOffset = 4.75f;

    [Header("Detection Settings")]
    [Tooltip("The radius used to check for nearby bees to avoid.")]
    [SerializeField] private float _separationCheckRadius = 2.5f;

    [Tooltip("The radius used to detect the target transform.")]
    [SerializeField] private float _targetCheckRadius = 10.0f;

    [Tooltip("Layer mask used to identify other swarm members.")]
    [SerializeField] private LayerMask _swarmLayers;

    [Header("Force Weighting")]
    [Tooltip("Strength modifier applied to the separation force calculation.")]
    [SerializeField] private float _separationForceMultiplier = 1.0f;

    [Tooltip("How much random Perlin noise affects movement jitter.")]
    [SerializeField] private float _perlinNoiseWeight = 0.35f;

    [Tooltip("Weight of the force pulling the bee toward the leader's position.")]
    [SerializeField] private float _directionToLeaderSwarmWeight = 1.25f;

    [Tooltip("Weight of the force aligning the bee with the leader's forward direction.")]
    [SerializeField] private float _leaderSwarmForwardWeight = 0.75f;

    [Tooltip("Weight of the force pushing bees away from each other.")]
    [SerializeField] private float _separationForceWeight = 1.5f;

    [Header("Component References")]
    [Tooltip("The detector component used for separation logic.")]
    [SerializeField] private OverlapSphereDetector _separationSphereDetector;

    [Tooltip("The detector component used for finding targets.")]
    [SerializeField] private OverlapSphereDetector _targetSearchSphereDetector;

    private Rigidbody _rb;
    private Swarm.State _currentState = Swarm.State.Idle;
    private TransformVariable _targetTransform;
    private List<Swarm> _swarmInstances;
    private UnityAction<Swarm> _containerSwarmDeathEvent;
    private NavMeshPath _path;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_separationSphereDetector == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Separation overlap sphere detector is not assigned in the inspector -");
        }
        else
        {
            _separationSphereDetector.SetRadius(_separationCheckRadius);
        }

        if (_targetSearchSphereDetector == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Target search overlap sphere detector is not assigned in the inspector -");
        }
        else
        {
            _targetSearchSphereDetector.SetRadius(_targetCheckRadius);
        }

        _path = new NavMeshPath();
    }

    public override void SetPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public override void SetData(
        List<Swarm> swarmObjects,
        TransformVariable targetTransform,
        UnityAction<Swarm> onSwarmDeathEvent,
        Swarm leaderSwarm
    )
    {
        _swarmInstances = swarmObjects;
        _targetTransform = targetTransform;
        _leaderSwarm = leaderSwarm;
        _containerSwarmDeathEvent = onSwarmDeathEvent;
    }

    public override void InvokeDeath()
    {
        _containerSwarmDeathEvent?.Invoke(this);
    }

    public override void StartChase()
    {
        _currentState = Swarm.State.Chase;
    }

    private Vector3 CalculateSwarmForce()
    {
        Vector3 leaderSwarmPosition = _leaderSwarm.transform.position;
        leaderSwarmPosition.y += _yOffset;

        Vector3 leaderSwarmDir = leaderSwarmPosition - transform.position;
        Vector3 leaderSwarmForward = _leaderSwarm.transform.forward;
        Vector3 perlinNoise = CalculateVectorPerlinNoise();
        Vector3 separationForce = Vector3.zero;

        float distanceToLeaderSwarm = (leaderSwarmPosition - transform.position).magnitude;

        List<Collider> swarmColliders = _separationSphereDetector.GetColliders(_swarmLayers);

        foreach (Collider swarmCollider in swarmColliders)
        {
            if (swarmCollider.gameObject == this.gameObject)
                continue;

            if (!_swarmInstances.Select(instance => instance.gameObject).Contains(swarmCollider.gameObject))
                continue;

            Vector3 diff = swarmCollider.gameObject.transform.position - transform.position;
            float distance = diff.magnitude;

            separationForce += _separationForceMultiplier * (diff.normalized * -1) / Mathf.Max(1.0f, distance);
        }

        Vector3 targetForce = (leaderSwarmDir.normalized * _directionToLeaderSwarmWeight) + (leaderSwarmForward * _leaderSwarmForwardWeight) + (perlinNoise * _perlinNoiseWeight) + (separationForce * _separationForceWeight);

        return targetForce.normalized;
    }

    private Vector3 CalculateVectorPerlinNoise()
    {
        Vector3 perlinNoise = new Vector3(
            Mathf.PerlinNoise(Time.time, transform.position.x) - 0.5f,
            Mathf.PerlinNoise(Time.time, transform.position.y) - 0.5f,
            Mathf.PerlinNoise(Time.time, transform.position.z) - 0.5f
        );

        return perlinNoise;
    }

    private Vector3 CalculateNextWaypoint(Vector3 origin, Vector3 targetPosition, NavMeshPath path)
    {
        NavMesh.CalculatePath(
            origin,
            targetPosition,
            NavMesh.AllAreas,
            path
        );

        if (path == null)
            return transform.position;

        if (path.corners.Length > 1)
            return path.corners[1];

        return transform.position;
    }

    public override void SwarmTick()
    {
        if (_leaderSwarm == null)
            return;

        switch (_currentState)
        {
            case Swarm.State.Idle:
                break;

            case Swarm.State.Chase:
                if (this.gameObject == _leaderSwarm.gameObject)
                {
                    Vector3 nextWaypoint = CalculateNextWaypoint(
                        transform.position,
                        _targetTransform.Value.position,
                        _path
                    );
                    Vector3 dir = nextWaypoint - transform.position;
                    _rb.linearVelocity = dir.normalized * _leaderSpeed;
                    break;
                }
                else
                {
                    _rb.linearVelocity = CalculateSwarmForce() * _swarmSpeed;
                    break;
                }
        }
    }

    private void OnDrawGizmos()
    {
        if (_leaderSwarm == null)
            return;

        if (this.gameObject != _leaderSwarm.gameObject)
            return;

        if (_path == null)
            return;

        if (_path.corners.Length == 0)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLineStrip(_path.corners, false);

        Gizmos.color = Color.yellow;
        foreach (Vector3 corner in _path.corners)
            Gizmos.DrawWireSphere(corner, 0.25f);
    }
}