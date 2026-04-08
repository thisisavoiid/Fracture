using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class OverlapBoxDetector : MonoBehaviour
{
    [SerializeField] private Vector3 _boxDimensions;
    [SerializeField] private Vector3 _boxOffset;

    public bool CheckForObjectsOfType<T>(LayerMask layerMask)
    {
        List<Collider> results = Physics.OverlapBox(
            transform.position + _boxOffset,
            _boxDimensions,
            Quaternion.identity,
            layerMask
        )
        .Where(obj => obj.GetComponent<T>() != null)
        .ToList();

        if (results == null || results.Count == 0)
            return false;

        return true;
    }

    public bool CheckForAnyObjects(LayerMask layerMask)
    {
        Collider[] results = Physics.OverlapBox(
            transform.position + _boxOffset,
            _boxDimensions,
            Quaternion.identity,
            layerMask
        );

        if (results == null || results.Length == 0)
            return false;

        return true;
    }

    public Collider[] GetColliders(LayerMask layerMask)
    {
        List<Collider> results = Physics.OverlapBox(
            transform.position + _boxOffset,
            _boxDimensions,
            Quaternion.identity,
            layerMask
        ).ToList();

        if (results == null || results.Count == 0)
            return null;

        return results.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            transform.position + _boxOffset,
            _boxDimensions
        );
    }

}