
using System.Collections;
using UnityEngine;

public class MeshFraction : MonoBehaviour
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private Transform _transform;
    private Vector3 _baseSize;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Explosion(float explosionForce)
    {
        bool isActive = gameObject.activeInHierarchy;

        if (!isActive)
            gameObject.SetActive(true); 

        _baseSize = _transform.localScale; 
        _rb.AddForce(UnityEngine.Random.onUnitSphere * explosionForce, ForceMode.Impulse);

        Destroy(gameObject, Random.Range(1.0f, 3.5f));
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (!collision.gameObject.CompareTag("floor"))
    //         return;

    //     Destroy(_boxCollider);
    //     _rb.isKinematic = true;

    // }
}