using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the lifecycle and collective behavior of a swarm group, including spawning, 
/// leader assignment, and target tracking.
/// </summary>
[RequireComponent(typeof(OverlapSphereDetector))]
public class SwarmContainerController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The number of swarm members to spawn (in addition to the leader).")]
    [Range(1, 30)]
    [SerializeField] private int _amount;

    [Tooltip("The dimensions of the area where swarm members will initially spawn.")]
    [SerializeField] private Vector3 _spawnContainerSize;

    [Tooltip("The prefab used to instantiate each swarm member.")]
    [SerializeField] private Swarm _swarmPrefab;

    [Header("Targeting & Detection")]
    [Tooltip("ScriptableObject or reference containing the target's transform data.")]
    [SerializeField] private TransformVariable _targetTransform;

    [Tooltip("Layers that, when detected, will trigger the swarm to start chasing.")]
    [SerializeField] private LayerMask _attackTriggerLayers;

    [Header("Leader Configuration")]
    [Tooltip("The name of the layer to assign to the leader to make them uninteractable/invincible.")]
    [SerializeField] private string _invincibleLayerName;

    [Tooltip("Layers that the leader swarm member is allowed to pass through without collision.")]
    [SerializeField] private LayerMask _leaderPassthroughLayers;

    private UnityAction<Swarm> OnSwarmDeath;
    private List<Swarm> _swarmInstances = new();
    private List<Vector3> _startPositions = new();
    private Swarm _currentLeaderSwarm;
    private OverlapSphereDetector _targetCheckDetector;

    private void Awake()
    {
        OnSwarmDeath += (swarm) => RemoveSwarm(swarm);
        _targetCheckDetector = GetComponent<OverlapSphereDetector>();

        CalculateStartPositions();
        SpawnSwarmObjects();
        SetLeaderSwarm();
        SetSwarmData();
        MoveSwarmsToDefaultPositions();
    }

    /// <summary>
    /// Spawns the specified amount of swarm members based on the prefab.
    /// </summary>
    private void SpawnSwarmObjects()
    {
        if (_swarmPrefab == null)
        {
            Debug.LogWarning($"[SWARM CONTAINER CONTROLLER] Swarm prefab is null, therefore, nothing can be instantiated -");
            return;
        }

        for (int i = 0; i < _amount + 1; i++)
        {
            Swarm swarmInstance = Instantiate(_swarmPrefab);
            swarmInstance.gameObject.name = $"{_swarmPrefab.gameObject.name}_{i + 1}";
            _swarmInstances.Add(swarmInstance);
        }
    }

    /// <summary>
    /// Positions each swarm member at its pre-calculated starting position.
    /// </summary>
    private void MoveSwarmsToDefaultPositions()
    {
        for (int i = 0; i < _swarmInstances.Count; i++)
            _swarmInstances[i].SetPosition(_startPositions[i]);
    }

    /// <summary>
    /// Passes necessary references and data to each swarm instance.
    /// </summary>
    private void SetSwarmData()
    {
        for (int i = 0; i < _swarmInstances.Count; i++)
            _swarmInstances[i].SetData(
                _swarmInstances,
                _targetTransform,
                OnSwarmDeath,
                _currentLeaderSwarm
            );
    }

    /// <summary>
    /// Randomly designates one swarm member as an invisible, invincible leader for others to follow.
    /// </summary>
    private void SetLeaderSwarm()
    {
        if (_swarmInstances.Count == 0)
            return;

        if (_swarmInstances.Count == 1)
        {
            _currentLeaderSwarm = _swarmInstances[0];
            return;
        }

        _currentLeaderSwarm = _swarmInstances[Random.Range(0, _swarmInstances.Count)];
        _currentLeaderSwarm.gameObject.layer = LayerMask.NameToLayer(_invincibleLayerName);

        // Hide leader visual and configure collision passthrough
        if (_currentLeaderSwarm.TryGetComponent(out MeshRenderer mr)) mr.enabled = false;
        if (_currentLeaderSwarm.TryGetComponent(out Collider col)) col.excludeLayers = _leaderPassthroughLayers;

        Light[] lights = _currentLeaderSwarm.GetComponentsInChildren<Light>();
        if (lights.Length > 0)
            foreach (Light light in lights)
                light.enabled = false;
    }

    /// <summary>
    /// Removes a swarm member from the simulation and handles group cleanup.
    /// </summary>
    public void RemoveSwarm(Swarm swarm)
    {
        _swarmInstances.Remove(swarm);

        if (swarm.gameObject != null)
            Destroy(swarm.gameObject);

        if (_swarmInstances.Count == 1)
        {
            Destroy(_swarmInstances[0].gameObject);
            _swarmInstances.RemoveAt(0);
        }
    }

    /// <summary>
    /// Generates random start positions within the defined spawn container area.
    /// </summary>
    private void CalculateStartPositions()
    {
        for (int i = 0; i < _amount + 1; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-_spawnContainerSize.x / 2, _spawnContainerSize.x / 2),
                Random.Range(-_spawnContainerSize.y / 2, _spawnContainerSize.y / 2),
                Random.Range(-_spawnContainerSize.z / 2, _spawnContainerSize.z / 2)
            );

            _startPositions.Add(transform.position + offset);
        }
    }

    /// <summary>
    /// Visualizes the spawn container and start positions in the Unity Editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, _spawnContainerSize);

        if (_startPositions == null)
            return;

        Gizmos.color = Color.green;
        foreach (Vector3 startPos in _startPositions)
        {
            Gizmos.DrawSphere(startPos, 0.075f);
        }
    }

    /// <summary>
    /// Regularly checks for targets and updates swarm behavior states.
    /// </summary>
    private void FixedUpdate()
    {
        bool isTargetInRange = _targetCheckDetector.CheckForAnyObjects(_attackTriggerLayers);
        foreach (Swarm swarm in _swarmInstances)
        {
            swarm.SwarmTick();

            if (isTargetInRange)
                swarm.StartChase();
        }
    }
}