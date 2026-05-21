using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RayCastDetector))]
public class FollowerBee : Bee
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _yOffset = 4.75f;
    [SerializeField] protected float _separationCheckRadius = 2.5f;
    [SerializeField] protected float _targetCheckRadius = 10.0f;
    [SerializeField] protected LayerMask _beeLayers;
    [SerializeField] protected float _separationForceMultiplier = 1.0f;
    [SerializeField] protected float _perlinNoiseWeight = 0.35f;
    [SerializeField] protected float _directionToLeaderSwarmWeight = 1.25f;
    [SerializeField] protected float _leaderSwarmForwardWeight = 0.75f;
    [SerializeField] protected float _separationForceWeight = 1.5f;
    [SerializeField] protected OverlapSphereDetector _separationSphereDetector;
    [SerializeField] protected OverlapSphereDetector _targetSearchSphereDetector;
    [SerializeField] protected Item _gun;
    [SerializeField] private float _deathZoneDistance;

    private RayCastDetector _rayCastDetector;
    private LeaderBee _leaderBee;
    private BeeState _currentState = BeeState.Idle;
    private TransformVariable _targetTransform;
    private List<FollowerBee> _followerBeeInstances;
    private Rigidbody _rb;
    private UnityAction<FollowerBee> _containerSwarmDeathEvent;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rayCastDetector = GetComponent<RayCastDetector>();

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

        if (_gun == null)
        {
            Debug.LogWarning($"[{this.GetType().Name.ToUpper()}] No Gun (Usable) assigned to {gameObject.name}. Bee will not be able to attack. -");
        }
    }

    public override void SetPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public void InjectData(
        List<FollowerBee> swarmObjects,
        TransformVariable targetTransform,
        UnityAction<FollowerBee> onSwarmDeathEvent,
        LeaderBee leaderBee
    )
    {
        _followerBeeInstances = swarmObjects;
        _targetTransform = targetTransform;
        _containerSwarmDeathEvent = onSwarmDeathEvent;
        _leaderBee = leaderBee;
        Debug.Log($"[{this.GetType().Name.ToUpper()}] Data set for {gameObject.name}. Leader is: {(leaderBee != null ? leaderBee.name : "NULL")} -");
    }

    public void InvokeDeathEvent()
    {
        Debug.Log($"[{this.GetType().Name.ToUpper()}] InvokeDeathEvent called for {gameObject.name}. -");
        _containerSwarmDeathEvent?.Invoke(this);
    }

    public override void SetState(BeeState state)
    {
        if (_currentState == state)
            return;

        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} changing state from {_currentState} to {state}. -");
        _currentState = state;
    }

    private Vector3 GetLeaderBeePosition()
    {
        if (_leaderBee == null)
            return Vector3.zero;

        return _leaderBee.transform.position;
    }

    private Vector3 GetDirection(Vector3 start, Vector3 end, bool normalized = true)
    {
        Vector3 dir = end - start;
        return normalized ? dir.normalized : dir;
    }

    private Vector3 GetLeaderBeeForward()
    {
        if (_leaderBee == null)
            return Vector3.zero;

        return _leaderBee.transform.forward;
    }

    private float GetDistance(Vector3 from, Vector3 to) => (to - from).magnitude;

    private Vector3 CalculateSwarmForce()
    {
        if (_leaderBee == null) return Vector3.zero;

        Vector3 leaderBeePos = GetLeaderBeePosition();
        leaderBeePos.y += _yOffset;

        Vector3 leaderBeeDir = GetDirection(transform.position, leaderBeePos);
        Vector3 leaderBeeForward = GetLeaderBeeForward();
        Vector3 perlinNoise = CalculateVectorPerlinNoise();
        Vector3 separationForce = Vector3.zero;

        List<Collider> beeColliders = _separationSphereDetector.GetColliders(_beeLayers);



        if (beeColliders != null && beeColliders.Count != 0)
        {
            foreach (Collider beeCollider in beeColliders)
            {
                if (beeCollider.gameObject == this.gameObject)
                    continue;

                if (!_followerBeeInstances.Select(instance => instance.gameObject).Contains(beeCollider.gameObject))
                    continue;

                Vector3 diff = beeCollider.gameObject.transform.position - transform.position;

                float distance = GetDistance(
                    transform.position,
                    beeCollider.gameObject.transform.position
                );

                separationForce += _separationForceMultiplier * (diff.normalized * -1) / Mathf.Max(1.0f, distance);
            }
        }

        Vector3 targetForce = (leaderBeeDir.normalized * _directionToLeaderSwarmWeight) +
                             (leaderBeeForward * _leaderSwarmForwardWeight) +
                             (perlinNoise * _perlinNoiseWeight) +
                             (separationForce * _separationForceWeight);

        return targetForce.normalized;
    }

    private Vector3 CalculateVectorPerlinNoise()
    {
        Vector3 perlinNoise = new Vector3(
            Mathf.PerlinNoise(Time.time * 0.25f, transform.position.x) - 0.5f,
            Mathf.PerlinNoise(Time.time * 0.25f, transform.position.y) - 0.5f,
            Mathf.PerlinNoise(Time.time * 0.25f, transform.position.z) - 0.5f
        );

        return perlinNoise;
    }

    private bool IsTargetInLineOfSight(TransformVariable targetTransform)
    {
        Vector3 targetDir = GetDirection(transform.position, targetTransform.Value.position);
        bool isObjectInLineOfSight = _rayCastDetector.Check(transform.position, targetDir, out RaycastHit hit, _targetCheckRadius);

        if (!isObjectInLineOfSight || hit.collider == null)
            return false;

        if (hit.collider.gameObject != targetTransform.Value.gameObject)
            return false;

        return true;
    }

    public override void Tick()
    {
        if (_leaderBee == null) return;

        Vector3 leaderBeePos = GetLeaderBeePosition();
        leaderBeePos.y = 0;

        Vector3 thisPos = transform.position;
        thisPos.y = 0;

        Vector3 targetForce;

        switch (_currentState)
        {
            case BeeState.Idle:
                targetForce = CalculateVectorPerlinNoise();
                _rb.linearVelocity = targetForce;

                break;

            case BeeState.Chase:
                bool isBeeInsideDeathZone = GetDistance(transform.position, GetLeaderBeePosition()) <= _deathZoneDistance;

                if (isBeeInsideDeathZone)
                    targetForce = CalculateVectorPerlinNoise();
                else
                    targetForce = CalculateSwarmForce() * _speed;

                _rb.linearVelocity = targetForce;

                if (_targetTransform == null || _targetTransform.Value == null) break;

                float distanceToTarget = GetDistance(transform.position, _targetTransform.Value.position);
                Vector3 targetDir = GetDirection(transform.position, _targetTransform.Value.position);

                if (distanceToTarget < _targetCheckRadius)
                {
                    bool canSeeTarget = IsTargetInLineOfSight(_targetTransform);

                    if (canSeeTarget && _gun != null)
                    {
                        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} is firing at {_targetTransform.Value.gameObject.name}! -");
                        
                        ItemUsageData usageData = new ItemUsageData(
                            transform.position,
                            targetDir,
                            true,
                            false
                        );

                        _gun.Use(usageData);
                    }
                }

                break;
        }
    }
}