using UnityEngine;

/// <summary>
/// Provides a wrapper for physics-based movement and rotation using the <see cref="Rigidbody"/> component.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement : MonoBehaviour
{
    [SerializeField] private bool _isLocked = false;

    public Vector3 CurrentVelocity => _rb.linearVelocity;

    private Rigidbody _rb;
    private Transform _transform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    /// <summary>
    /// Moves the body in a specified direction while maintaining current vertical velocity.
    /// </summary>
    /// <param name="dir">The normalized direction of movement.</param>
    /// <param name="speed">The scalar speed applied to the direction.</param>
    public void Move(Vector3 dir, float speed, bool preserveYVelocity)
    {
        if (_isLocked)
            return;

        Vector3 currentVelocity = _rb.linearVelocity;
        Vector3 targetVelocity = dir.normalized * speed;

        // Preserve gravity or existing vertical momentum
        if (preserveYVelocity)
            targetVelocity.y = currentVelocity.y;

        _rb.linearVelocity = targetVelocity;
    }

    /// <summary>
    /// Applies an upward impulse to the <see cref="Rigidbody"/> to simulate a jump.
    /// </summary>
    /// <param name="strength">The desired target vertical velocity.</param>
    /// <remarks>
    /// The force applied is the difference between the <paramref name="strength"/> 
    /// and the current upward velocity to ensure consistent jump heights.
    /// </remarks>
    public void Jump(float strength)
    {
        if (_isLocked)
            return;

        float currentVerticalForce = _rb.linearVelocity.y;
        float difference = Mathf.Max(0, strength - currentVerticalForce);

        _rb.AddForce(_transform.up * difference, ForceMode.Impulse);

        Debug.Log($"[RIGIDBODY MOVEMENT] Applying jump force to {gameObject.name} with force: {_transform.up * difference} -");
    }

    /// <summary>
    /// Gets the current world-space rotation.
    /// </summary>
    public Quaternion GetRotation() => _transform.rotation;

    /// <summary>
    /// Updates the world-space rotation using <see cref="Rigidbody.MoveRotation(Quaternion)"/> 
    /// to maintain proper physics interpolation.
    /// </summary>
    /// <param name="rotation">The target rotation.</param>
    public void SetRotation(Quaternion rotation)
    {
        if (_isLocked)
            return;

        _rb.MoveRotation(rotation);
    }

    /// <summary>
    /// Gets the current local-space rotation.
    /// </summary>
    public Quaternion GetLocalRotation() => _transform.localRotation;

    /// <summary>
    /// Sets the local-space rotation directly via the <see cref="Transform"/>.
    /// </summary>
    /// <param name="rotation">The target local rotation.</param>
    public void SetLocalRotation(Quaternion rotation)
    {
        if (_isLocked)
            return;

        _transform.localRotation = rotation;
    }

    public void SetKinematic(bool value) => _rb.isKinematic = value;
    public void SetUseGravity(bool value) => _rb.useGravity = value;
    
    public void SetLocked(bool value) => _isLocked = value;
}