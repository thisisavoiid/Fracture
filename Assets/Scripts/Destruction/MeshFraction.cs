using System.Collections;
using UnityEngine;

public class MeshFraction : MonoBehaviour
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private Transform _transform;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Explosion(float explosionForce, Vector3 origin, float minRandomDestroyTime, float maxRandomDestroyTime)
    {
        bool isActive = gameObject.activeInHierarchy;

        if (!isActive)
            gameObject.SetActive(true); 

        Vector3 dir = (_transform.position - origin).normalized;

        _rb.AddForce(explosionForce * dir, ForceMode.Impulse);

        Destroy(gameObject, Random.Range(minRandomDestroyTime, maxRandomDestroyTime));
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (!collision.gameObject.CompareTag("floor"))
    //         return;

    //     Destroy(_boxCollider);
    //     _rb.isKinematic = true;

    // }
}