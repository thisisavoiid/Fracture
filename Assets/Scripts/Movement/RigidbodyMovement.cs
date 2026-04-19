using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _transform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    public void Move(Vector3 dir, float speed)
    {
        Vector3 currentVelocity = _rb.linearVelocity;
        Vector3 targetVelocity = dir.normalized * speed;
        targetVelocity.y = currentVelocity.y;

        _rb.linearVelocity = targetVelocity;
    }

    public void Jump(float strength)
    {
        float currentVerticalForce = _rb.linearVelocity.y;
        float difference = Mathf.Max(0, strength - currentVerticalForce);

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
