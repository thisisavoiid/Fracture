using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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

    private void MoveSwarmsToDefaultPositions()
    {
        for (int i = 0; i < _swarmInstances.Count; i++)
            _swarmInstances[i].SetPosition(_startPositions[i]);
    }

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
        
        MeshRenderer leaderMeshRenderer = _currentLeaderSwarm.GetComponent<MeshRenderer>();
        if (leaderMeshRenderer != null)
            leaderMeshRenderer.enabled = false;

        Collider leaderSwarmCollider = _currentLeaderSwarm.GetComponent<Collider>();
        if (leaderSwarmCollider != null)
            leaderSwarmCollider.excludeLayers = _leaderPassthroughLayers;
    }

    public void RemoveSwarm(Swarm swarm)
    {
        _swarmInstances.Remove(swarm);
    }

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