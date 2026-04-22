using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class OverlapSphereDetector : MonoBehaviour
{
    [SerializeField] private float _sphereRadius;
    [SerializeField] private Vector3 _sphereOffset;

    public bool CheckForObjectsOfType<T>(LayerMask layerMask)
    {
        List<Collider> results = Physics.OverlapSphere(
            transform.position + _sphereOffset,
            _sphereRadius,
            layerMask
        )
        .Where(obj => obj.GetComponent<T>() != null)
        .ToList();

        if (results == null || results.Count == 0)
            return false;

        return true;
    }

    public void SetRadius(float radius) => _sphereRadius = radius;
    
    public bool CheckForAnyObjects(LayerMask layerMask)
    {
        Collider[] results = Physics.OverlapSphere(
            transform.position + _sphereOffset,
            _sphereRadius,
            layerMask
        );

        if (results == null || results.Length == 0)
            return false;

        return true;
    }

    public List<Collider> GetColliders(LayerMask layerMask)
    {
        List<Collider> results = Physics.OverlapSphere(
            transform.position + _sphereOffset,
            _sphereRadius,
            layerMask
        ).ToList();

        if (results == null || results.Count == 0)
            return null;

        return results;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position + _sphereOffset,
            _sphereRadius
        );
    }

}