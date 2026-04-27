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
    [SerializeField] private LayerMask _swarmLayers;
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
        SetNewLeaderSwarm();
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

        for (int i = 0; i < _amount; i++)
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
                _swarmLayers,
                _targetTransform,
                OnSwarmDeath,
                _currentLeaderSwarm
            );
    }

    private void SetNewLeaderSwarm()
    {
        if (_swarmInstances.Count == 0)
            return;

        if (_swarmInstances.Count == 1)
        {
            _currentLeaderSwarm = _swarmInstances[0];
            return;
        }

        _currentLeaderSwarm = _swarmInstances[Random.Range(0, _swarmInstances.Count - 1)];
    }
    public void RemoveSwarm(Swarm swarm)
    {
        _swarmInstances.Remove(swarm);

        if (swarm == _currentLeaderSwarm)
        {
            SetNewLeaderSwarm();
            SetSwarmData();
        }

    }

    private void CalculateStartPositions()
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
        bool isTargetInRange = _targetCheckDetector.CheckForAnyObjects(_attackTriggerLayers);
        foreach (Swarm swarm in _swarmInstances)
        {
            swarm.SwarmTick();

            if (isTargetInRange)
                swarm.StartChase();
        }
    }
}
