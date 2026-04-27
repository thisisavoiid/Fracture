using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(OverlapSphereDetector))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class BeeBrain : Swarm
{
    [Header("Debugging")]
    [SerializeField] private Swarm _leaderSwarm;
    [SerializeField] private bool _showNeighbourBees = false;

    [Header("Settings")]
    [SerializeField] private float _separationForceMultiplier = 1.0f;
    [SerializeField] private float _separationCheckRadius = 2.5f;
    [SerializeField] private float _targetCheckRadius = 10.0f;
    [SerializeField] private float _swarmSpeed;
    [SerializeField] private float _leaderSpeed;

    [Header("Force Weighting")]
    [SerializeField] private float _perlinNoiseWeight = 0.35f;
    [SerializeField] private float _directionToLeaderSwarmWeight = 1.25f;
    [SerializeField] private float _leaderSwarmForwardWeight = 0.75f;
    [SerializeField] private float _separationForceWeight = 1.5f;

    [Header("Target Detection")]
    [SerializeField] private OverlapSphereDetector _separationSphereDetector;
    [SerializeField] private OverlapSphereDetector _targetSearchSphereDetector;

    private Rigidbody _rb;
    private Swarm.State _currentState = Swarm.State.Idle;
    private TransformVariable _targetTransform;
    private List<Swarm> _swarmInstances;
    private LayerMask _swarmLayers;
    private Health _health;
    private UnityAction<Swarm> _containerSwarmDeathEvent;

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

        _health = GetComponent<Health>();
    }
    public override void SetPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public override void SetData(
        List<Swarm> swarmObjects,
        LayerMask swarmLayers,
        TransformVariable targetTransform,
        UnityAction<Swarm> onSwarmDeathEvent,
        Swarm leaderSwarm
    )
    {
        _swarmInstances = swarmObjects;
        _swarmLayers = swarmLayers;
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
        Vector3 leaderSwarmDir = leaderSwarmPosition - transform.position;
        Vector3 leaderSwarmForward = _leaderSwarm.transform.forward;

        Vector3 perlinNoise = CalculateVectorPerlinNoise();

        float distanceToLeaderSwarm = (leaderSwarmPosition - transform.position).magnitude;

        List<Collider> swarmColliders = _separationSphereDetector.GetColliders(_swarmLayers);

        Vector3 separationForce = Vector3.zero;

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
                    Vector3 dir = _targetTransform.Value.position - transform.position;
                    _rb.linearVelocity = (dir.normalized + CalculateVectorPerlinNoise()) * _leaderSpeed ;
                    break;
                }
                else
                {
                    _rb.linearVelocity = CalculateSwarmForce() * _swarmSpeed;
                    break;
                }
        }
    }
}