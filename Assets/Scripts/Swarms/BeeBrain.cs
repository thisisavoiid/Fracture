using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Controls individual bee behavior within a swarm, utilizing a flocking algorithm 
/// (Separation, Alignment, and Cohesion towards a leader) combined with NavMesh pathfinding 
/// for the leader bee and combat logic for followers.
/// </summary>
[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(Timer))]
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

    [Tooltip("The weapon component used by the bee to attack targets.")]
    [SerializeField] private Usable _gun;

    private Rigidbody _rb;
    private Swarm.State _currentState = Swarm.State.Idle;
    private TransformVariable _targetTransform;
    private List<Swarm> _swarmInstances;
    private UnityAction<Swarm> _containerSwarmDeathEvent;
    private NavMeshPath _path;
    private RayCastDetector _rayCastDetector;
    private Timer _timer;

    /// <summary>
    /// Initializes internal references and validates inspector assignments for detectors.
    /// </summary>
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
        _timer = GetComponent<Timer>();
        _rayCastDetector = GetComponent<RayCastDetector>();

        if (_gun == null)
        {
            Debug.LogWarning($"[{this.GetType().Name.ToUpper()}] No Gun (Usable) assigned to {gameObject.name}. Bee will not be able to attack. -");
        }
    }

    /// <summary>
    /// Teleports the bee to the specified starting position.
    /// </summary>
    public override void SetPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    /// <summary>
    /// Injects swarm-wide data including the list of peers, the target transform, and the death callback.
    /// </summary>
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

        Debug.Log($"[{this.GetType().Name.ToUpper()}] Data set for {gameObject.name}. Leader is: {(_leaderSwarm != null ? _leaderSwarm.name : "NULL")} -");
    }

    /// <summary>
    /// Notifies the swarm container of this individual's death.
    /// </summary>
    public override void InvokeDeath()
    {
        Debug.Log($"[{this.GetType().Name.ToUpper()}] InvokeDeath called for {gameObject.name}. -");
        _containerSwarmDeathEvent?.Invoke(this);
    }

    /// <summary>
    /// Transitions the bee's state to Chase, enabling pursuit logic.
    /// </summary>
    public override void StartChase()
    {
        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} changing state from {_currentState} to Chase. -");
        _currentState = Swarm.State.Chase;
    }

    /// <summary>
    /// Calculates a composite steering force based on the leader's position, 
    /// noise for natural movement, and separation from other bees.
    /// </summary>
    /// <returns>A normalized direction vector representing the swarm force.</returns>
    private Vector3 CalculateSwarmForce()
    {
        if (_leaderSwarm == null) return Vector3.zero;

        Vector3 leaderSwarmPosition = _leaderSwarm.transform.position;
        leaderSwarmPosition.y += _yOffset;

        Vector3 leaderSwarmDir = leaderSwarmPosition - transform.position;
        Vector3 leaderSwarmForward = _leaderSwarm.transform.forward;
        Vector3 perlinNoise = CalculateVectorPerlinNoise();
        Vector3 separationForce = Vector3.zero;

        List<Collider> swarmColliders = _separationSphereDetector.GetColliders(_swarmLayers);

        if (swarmColliders != null && swarmColliders.Count != 0)
        {
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
        }

        Vector3 targetForce = (leaderSwarmDir.normalized * _directionToLeaderSwarmWeight) +
                             (leaderSwarmForward * _leaderSwarmForwardWeight) +
                             (perlinNoise * _perlinNoiseWeight) +
                             (separationForce * _separationForceWeight);

        return targetForce.normalized;
    }

    /// <summary>
    /// Generates a pseudo-random jitter vector using Perlin noise for smoother, more organic movement.
    /// </summary>
    private Vector3 CalculateVectorPerlinNoise()
    {
        Vector3 perlinNoise = new Vector3(
            Mathf.PerlinNoise(Time.time, transform.position.x) - 0.5f,
            Mathf.PerlinNoise(Time.time, transform.position.y) - 0.5f,
            Mathf.PerlinNoise(Time.time, transform.position.z) - 0.5f
        );

        return perlinNoise;
    }

    /// <summary>
    /// Calculates the next valid NavMesh corner towards the target position.
    /// </summary>
    private Vector3 CalculateNextWaypoint(Vector3 origin, Vector3 targetPosition, NavMeshPath path)
    {
        if (!NavMesh.CalculatePath(origin, targetPosition, NavMesh.AllAreas, path))
        {
            Debug.LogWarning($"[{this.GetType().Name.ToUpper()}] NavMesh path calculation failed for {gameObject.name} -");
        }

        if (path == null)
            return transform.position;

        if (path.corners.Length > 1)
            return path.corners[1];

        return transform.position;
    }

    /// <summary>
    /// Called every tick to process movement and combat logic based on the current <see cref="Swarm.State"/>.
    /// </summary>
    public override void SwarmTick()
    {
        if (_leaderSwarm == null)
            return;

        switch (_currentState)
        {
            case Swarm.State.Idle:
                // Followers drift around the leader in idle
                if (this.gameObject != _leaderSwarm.gameObject)
                {
                    _rb.linearVelocity = CalculateSwarmForce() * _swarmSpeed;
                }
                break;

            case Swarm.State.Chase:
                if (this.gameObject == _leaderSwarm.gameObject)
                {
                    // Leader follows NavMesh pathing
                    Vector3 nextWaypoint = CalculateNextWaypoint(
                        transform.position,
                        _targetTransform.Value.position,
                        _path
                    );
                    Vector3 dir = nextWaypoint - transform.position;
                    _rb.linearVelocity = dir.normalized * _leaderSpeed;
                }
                else
                {
                    // Followers steer toward leader and attack target if in range
                    _rb.linearVelocity = CalculateSwarmForce() * _swarmSpeed;

                    if (_targetTransform == null || _targetTransform.Value == null) break;

                    float distanceToTarget = (_targetTransform.Value.position - transform.position).magnitude;
                    Vector3 targetDir = _targetTransform.Value.position - transform.position;

                    if (distanceToTarget < _targetCheckRadius)
                    {
                        bool isObjectInLineOfSight = _rayCastDetector.Check(transform.position, targetDir.normalized, out RaycastHit hit, _targetCheckRadius);

                        if (!isObjectInLineOfSight || hit.collider == null)
                            break;

                        if (_gun == null)
                            break;

                        if (hit.collider.gameObject != _targetTransform.Value.gameObject)
                            break;

                        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} is firing at {_targetTransform.Value.gameObject.name}! -");
                        _gun.Use(transform.position, targetDir, true, false);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Renders the leader's NavMesh path and waypoints for debugging in the Editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_leaderSwarm == null || this.gameObject != _leaderSwarm.gameObject)
            return;

        if (_path == null || _path.corners.Length == 0)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLineStrip(_path.corners, false);

        Gizmos.color = Color.yellow;
        foreach (Vector3 corner in _path.corners)
            Gizmos.DrawWireSphere(corner, 0.25f);
    }
}