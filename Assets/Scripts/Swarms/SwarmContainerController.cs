using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(OverlapSphereDetector))]

public class SwarmContainerController : MonoBehaviour
{
    [SerializeField] private int _amount;
    [SerializeField] private Vector3 _spawnContainerSize;
    [SerializeField] private Swarm _swarmPrefab;
    [SerializeField] private TransformVariable _targetTransform;
    [SerializeField] private LayerMask _attackTriggerLayers;

    private UnityAction<Swarm> OnSwarmDeath;
    private UnityEvent<Swarm> OnSwarmLeaderChange;
    private List<Swarm> _swarmInstances = new();
    private List<Vector3> _startPositions = new();
    private OverlapSphereDetector _overlapSphereDetector;
    private Swarm _swarmLeader;

    private void Awake()
    {
        OnSwarmDeath += (swarm) => RemoveSwarm(swarm);
        _overlapSphereDetector = GetComponent<OverlapSphereDetector>();

        SetStartPositions();

        if (_swarmPrefab == null)
        {
            Debug.LogWarning($"[SWARM CONTAINER CONTROLLER] Swarm prefab is null, therefore, nothing can be instantiated -");
            return;
        }

        for (int i = 0; i < _amount; i++)
        {
            Swarm swarmInstance = Instantiate(_swarmPrefab);
            swarmInstance.gameObject.name = $"{_swarmPrefab.gameObject.name}_{i + 1}";
            _swarmInstances.Add(swarmInstance);
        }

        _swarmLeader = _swarmInstances[Random.Range(0, _swarmInstances.Count - 1)];

        for (int i = 0; i < _swarmInstances.Count; i++)
        {
            SwarmContext ctx = new SwarmContext(
                OnSwarmDeath,
                _targetTransform,
                _startPositions[i],
                _swarmLeader
            );

            _swarmInstances[i].Init(ctx);

        }
    }

    public void RemoveSwarm(Swarm swarm)
    {
        _swarmInstances.Remove(swarm);
        if (swarm == _swarmLeader)
        {
            _swarmLeader = _swarmInstances[Random.Range(0, _swarmInstances.Count - 1)];
            OnSwarmLeaderChange?.Invoke(_swarmLeader);
        }
            
    }

    private void SetStartPositions()
    {
        for (int i = 0; i < _amount; i++)
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
        bool targetFound = _overlapSphereDetector.CheckForAnyObjects(_attackTriggerLayers);

        if (!targetFound)
            return;

        foreach (Swarm swarm in _swarmInstances)
        {
            if (!swarm.IsAttacking)
                swarm.StartAttack();
        }
    }
}
