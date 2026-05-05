using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Provides utility methods for detecting colliders within a spherical area using <see cref="Physics.OverlapSphere(Vector3, float, int)"/>.
/// </summary>
/// <remarks>
/// This component allows for easy spatial queries with a configurable offset and radius, 
/// including filtering by specific component types via <see cref="GameObject.GetComponent{T}()"/>.
/// </remarks>
public class OverlapSphereDetector : MonoBehaviour
{
    [Tooltip("The radius of the detection sphere.")]
    [SerializeField] private float _sphereRadius;

    [Tooltip("The local offset from the Transform position where the sphere center is located.")]
    [SerializeField] private Vector3 _sphereOffset;

    /// <summary>
    /// Checks if any colliders within the sphere contain a specific component type.
    /// </summary>
    /// <typeparam name="T">The type of component to look for on the detected colliders.</typeparam>
    /// <param name="layerMask">The <see cref="LayerMask"/> used to filter which layers are checked.</param>
    /// <returns>True if at least one object of type <typeparamref name="T"/> is found.</returns>
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

    /// <summary>
    /// Updates the <see cref="_sphereRadius"/> value at runtime.
    /// </summary>
    /// <param name="radius">The new radius for the detection sphere.</param>
    public void SetRadius(float radius) => _sphereRadius = radius;
    
    /// <summary>
    /// Checks for the presence of any colliders on the specified layers within the sphere.
    /// </summary>
    /// <param name="layerMask">The layers to include in the detection.</param>
    /// <returns>True if any collider is detected.</returns>
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

    /// <summary>
    /// Retrieves a list of all <see cref="Collider"/> components found within the sphere.
    /// </summary>
    /// <param name="layerMask">The layers to filter the search.</param>
    /// <returns>A list of <see cref="Collider"/> objects, or null if no objects are found.</returns>
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

    /// <summary>
    /// Draws a wireframe sphere in the Scene view to visualize the detection area.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position + _sphereOffset,
            _sphereRadius
        );
    }
}