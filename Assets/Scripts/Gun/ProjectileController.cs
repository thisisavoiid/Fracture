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
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_killAfterTime <= 0)
            return;

        StartCoroutine(ProjectileLifeCycle());
    }

    private IEnumerator ProjectileLifeCycle()
    {
        yield return new WaitForSeconds(_killAfterTime);
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveDir * _speed;
    }

    public void Init(Vector3 dir)
    {
        _moveDir = dir;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((_layerMask & (1 << collision.gameObject.layer)) != 0)
            Destroy(this.gameObject);
    }
}
