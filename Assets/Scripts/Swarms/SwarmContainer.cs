using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Manages the lifecycle and collective behavior of a swarm group, including spawning, 
/// leader assignment, and target tracking.
/// </summary>
[RequireComponent(typeof(OverlapSphereDetector))]
public class SwarmContainer : Spawnable
{
    [Range(1, 30)]
    [SerializeField] private int _amount;
    [SerializeField] private Vector3 _spawnContainerSize;
    [SerializeField] private FollowerBee _followerBeePrefab;
    [SerializeField] private LeaderBee _leaderBeePrefab;
    [SerializeField] private TransformVariable _targetTransform;
    [SerializeField] private LayerMask _attackTriggerLayers;

    private UnityAction<FollowerBee> OnSwarmDeath;
    private List<FollowerBee> _followerBeeInstances = new();
    private List<Vector3> _startPositions = new();
    private OverlapSphereDetector _targetCheckDetector;
    private LeaderBee _leaderBeeInstance;
    private bool _hasSwarmBeenAlertedBefore = false;

    public override void Spawn()
    {
        OnSwarmDeath += (followerBee) => RemoveFollowerBee(followerBee);
        _targetCheckDetector = GetComponent<OverlapSphereDetector>();

        CalculateStartPositions();
        InstantiateFollowerBees();
        InstantiateLeaderBee();
        SetFollowerBeeData();
        MoveSwarmsToDefaultPositions();
    }

    private void InstantiateFollowerBees()
    {
        if (_followerBeeInstances == null)
        {
            Debug.LogWarning($"[SWARM CONTAINER CONTROLLER] Swarm prefab is null, therefore, nothing can be instantiated -");
            return;
        }

        for (int i = 0; i < _amount; i++)
        {
            FollowerBee followerBeeInstance = Instantiate(_followerBeePrefab, transform);
            followerBeeInstance.gameObject.name = $"{_followerBeePrefab.gameObject.name}_{i + 1}";
            _followerBeeInstances.Add(followerBeeInstance);
        }
    }

    private void MoveSwarmsToDefaultPositions()
    {
        for (int i = 0; i < _followerBeeInstances.Count; i++)
            _followerBeeInstances[i].SetPosition(_startPositions[i]);
    }

    private void SetFollowerBeeData()
    {
        for (int i = 0; i < _followerBeeInstances.Count; i++)
            _followerBeeInstances[i].InjectData(
                _followerBeeInstances,
                _targetTransform,
                OnSwarmDeath,
                _leaderBeeInstance
            );
    }

    private void InstantiateLeaderBee()
    {
        if (_leaderBeePrefab == null)
            return;

        _leaderBeeInstance = Instantiate(_leaderBeePrefab, transform.position, Quaternion.identity);
        _leaderBeeInstance.gameObject.name = _leaderBeePrefab.name;

        _leaderBeeInstance.SetTargetTransform(_targetTransform);
    }

    public void RemoveFollowerBee(FollowerBee followerBee)
    {
        _followerBeeInstances.Remove(followerBee);

        if (followerBee.gameObject != null)
            Destroy(followerBee.gameObject);

        if (_followerBeeInstances.Count == 0)
        {
            Destroy(_leaderBeeInstance.gameObject);
            Destroy(this.gameObject);
        }
            
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

    private void HandleTicks()
    {
        _leaderBeeInstance.Tick();

        foreach (Bee swarm in _followerBeeInstances)
        {
            swarm.Tick();
        }
    }

    private void SetStateOfAllBees(BeeState state)
    {
        foreach (FollowerBee followerBee in _followerBeeInstances)
            followerBee.SetState(state);

        _leaderBeeInstance.SetState(state);
    }

    private void FixedUpdate()
    {
        HandleTicks();

        bool isTargetInRange = _targetCheckDetector.CheckForAnyObjects(_attackTriggerLayers);

        if (isTargetInRange && !_hasSwarmBeenAlertedBefore)
        {
            SetStateOfAllBees(BeeState.Chase);
            _hasSwarmBeenAlertedBefore = true;
        }
            
    }
}