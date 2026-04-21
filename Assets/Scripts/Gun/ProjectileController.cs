using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _speed;
    private Transform _transform;
    private RigidbodyMovement _rbMovement;
    private Vector3 _moveDir;
    private Rigidbody _rb;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(_moveDir * _speed, ForceMode.Force);
    }

    public void Init(Vector3 dir)
    {
        _moveDir = dir;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((_layerMask & (1 << other.gameObject.layer)) != 0)
            Destroy(this.gameObject);
    }
}
