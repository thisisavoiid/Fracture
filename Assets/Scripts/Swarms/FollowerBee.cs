using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RayCastDetector))]
public class FollowerBee : Bee, ICollectionMember
{
    [SerializeField] private FollowerBeeConfig _config;

    [SerializeField]
    [BoxGroup("Initialization Endpoint")]
    private UnityEvent _onBeeInitialize;

    private RayCastDetector _rayCastDetector;
    private LeaderBee _leaderBee;
    private BeeState _currentState = BeeState.Idle;
    private TransformVariable _targetTransform;
    private List<FollowerBee> _followerBeeInstances;
    private Rigidbody _rb;
    private UnityAction<FollowerBee> _containerSwarmDeathEvent;
    private Quaternion _targetRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rayCastDetector = GetComponent<RayCastDetector>();

        if (_config.SeparationSphereDetector == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Separation overlap sphere detector is not assigned in the inspector -");
        }
        else
        {
            _config.SeparationSphereDetector.SetRadius(_config.SeparationCheckRadius);
        }

        if (_config.TargetSearchSphereDetector == null)
        {
            Debug.LogError($"[{this.GetType().Name.ToUpper()}] Target search overlap sphere detector is not assigned in the inspector -");
        }
        else
        {
            _config.TargetSearchSphereDetector.SetRadius(_config.TargetCheckRadius);
        }

        if (_config.Gun == null)
        {
            Debug.LogWarning($"[{this.GetType().Name.ToUpper()}] No Gun (Usable) assigned to {gameObject.name}. Bee will not be able to attack. -");
        }
    }

    private void Start()
    {
        _onBeeInitialize?.Invoke();
        Subscribe();
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
        Unsubscribe();
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
        leaderBeePos.y += _config.YOffset;

        Vector3 leaderBeeDir = GetDirection(transform.position, leaderBeePos);
        Vector3 leaderBeeForward = GetLeaderBeeForward();
        Vector3 perlinNoise = CalculateVectorPerlinNoise();
        Vector3 separationForce = Vector3.zero;

        List<Collider> beeColliders = _config.SeparationSphereDetector.GetColliders(_config.BeeLayers);

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

                separationForce += _config.SeparationForceMultiplier * (diff.normalized * -1) / Mathf.Max(1.0f, distance);
            }
        }

        Vector3 targetForce = (leaderBeeDir.normalized * _config.DirectionToLeaderSwarmWeight) +
                             (leaderBeeForward * _config.LeaderSwarmForwardWeight) +
                             (perlinNoise * _config.PerlinNoiseWeight) +
                             (separationForce * _config.SeparationForceWeight);

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
        bool isObjectInLineOfSight = _rayCastDetector.Check(transform.position, targetDir, out RaycastHit hit, _config.TargetCheckRadius);

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
                bool isBeeCloseToTarget = GetDistance(transform.position, _targetTransform.Value.position) <= _config.TargetCheckRadius;
                bool isBeeInsideDeathZone = GetDistance(transform.position, GetLeaderBeePosition()) <= _config.DeathZoneDistance;

                if (isBeeInsideDeathZone || isBeeCloseToTarget)
                    targetForce = CalculateVectorPerlinNoise();
                else
                    targetForce = CalculateSwarmForce() * _config.Speed;

                _rb.linearVelocity = Vector3.Lerp(
                    _rb.linearVelocity,
                    targetForce,
                    Time.deltaTime * _config.Acceleration
                );

                if (_targetTransform == null || _targetTransform.Value == null) break;

                Vector3 targetDir = GetDirection(transform.position, _targetTransform.Value.position);

                if (targetDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.LookRotation(targetDir),
                        Time.deltaTime * _config.TurnToTargetSpeed
                    );

                    // Vector3 rotationEuler = targetRotation.eulerAngles;

                    // rotationEuler.x = Mathf.Clamp(rotationEuler.x, -_config.ClampDegrees, _config.ClampDegrees);

                    // transform.rotation = Quaternion.Euler(rotationEuler);
                    transform.rotation = targetRotation;
                }

                if (isBeeCloseToTarget)
                {
                    bool canSeeTarget = IsTargetInLineOfSight(_targetTransform);

                    if (canSeeTarget && _config.Gun != null)
                    {
                        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} is firing at {_targetTransform.Value.gameObject.name}! -");

                        ItemUsageData usageData = new ItemUsageData(
                            transform.position,
                            targetDir,
                            true,
                            false
                        );

                        _config.Gun.Use(usageData);
                    }
                }

                break;
        }
    }

    public void Subscribe()
    {
        EnemyCollectionManager.Instance?.Subscribe(this);
    }

    public void Unsubscribe()
    {
        EnemyCollectionManager.Instance?.Unsubscribe(this);
    }
}