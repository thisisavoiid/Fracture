using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _transform;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpStrength;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    public void Move(Vector3 dir)
    {
        Vector3 normalizedDir = dir.normalized;
        Vector3 targetDir = normalizedDir * _moveSpeed;

        _rb.MovePosition(_transform.position + targetDir * Time.deltaTime);
    }

    public void Jump()
    {
        float currentVerticalForce = _rb.linearVelocity.y;
        float difference = Mathf.Max(0, _jumpStrength - currentVerticalForce);

        _rb.AddForce(_transform.up * difference, ForceMode.Impulse);

        Debug.Log($"[RIGIDBODY MOVEMENT] Applying jump force to {gameObject.name} with force: {_transform.up * difference} -");
    }

    public Quaternion GetRotation() => _transform.rotation;
    public void SetRotation(Quaternion rotation)
    {
        _rb.MoveRotation(rotation);
    }

    public Quaternion GetLocalRotation() => _transform.localRotation;
    public void SetLocalRotation(Quaternion rotation)
    {
        _transform.localRotation = rotation;
    }

}
