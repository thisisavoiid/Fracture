using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class LeaderBee : Bee
{
    [SerializeField] private float _speed;
    private NavMeshPath _path;
    private Rigidbody _rb;
    private BeeState _currentState;
    private TransformVariable _targetTransform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _path = new NavMeshPath();
    }

    public void SetTargetTransform(TransformVariable targetTransformVariable)
    {
        if (targetTransformVariable == null)
            return;

        _targetTransform = targetTransformVariable;
    }

    public override void SetPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public override void SetState(BeeState state)
    {
        if (_currentState == state)
            return;
            
        Debug.Log($"[{this.GetType().Name.ToUpper()}] {gameObject.name} changing state from {_currentState} to {state}. -");
        _currentState = state;
    }

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

    public override void Tick()
    {
        if (_currentState != BeeState.Chase) return;

        Vector3 nextWaypoint = CalculateNextWaypoint(
            transform.position,
            _targetTransform.Value.position,
            _path
        );

        Vector3 dir = nextWaypoint - transform.position;
        _rb.linearVelocity = dir.normalized * _speed;
    }

    private void OnDrawGizmos()
    {
        if (_path == null || _path.corners.Length == 0)
            return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLineStrip(_path.corners, false);

        Gizmos.color = Color.yellow;
        foreach (Vector3 corner in _path.corners)
            Gizmos.DrawWireSphere(corner, 0.25f);
    }
}