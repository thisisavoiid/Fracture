using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _killAfterTime;
    [SerializeField] private float _speed;
    private Vector3 _moveDir;
    private Vector3 _origin;
    private float _range;
    private Rigidbody _rb;
    private bool _isProjectileLifeCycleRunning = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_killAfterTime <= 0)
            return;
    }

    private IEnumerator ProjectileLifeCycle()
    {
        if (_isProjectileLifeCycleRunning)
            yield return null;

        Debug.Log($"[PROJECTILE CONTROLLER] Started projectile lifecycle: Destroying after {_killAfterTime} seconds! -");
        _isProjectileLifeCycleRunning = true;
        yield return new WaitForSeconds(_killAfterTime);
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * _speed;

        float distanceToOrigin = (_origin - _rb.position).magnitude;

        if (distanceToOrigin >= _range)
            Destroy(this.gameObject);
    }

    public void Init(Vector3 dir, float range=Mathf.Infinity)
    {
        _moveDir = dir;
        _range = range;
        _origin = _rb.position;
        
        if (_range == Mathf.Infinity)
            StartCoroutine(ProjectileLifeCycle());
    }
}
