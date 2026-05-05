using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Provides utility methods for detecting colliders within a box-shaped area using <see cref="Physics.OverlapBox(Vector3, Vector3, Quaternion, int)"/>.
/// </summary>
/// <remarks>
/// This component facilitates spatial queries using a box volume with configurable dimensions and offsets, 
/// supporting type-based filtering via <see cref="GameObject.GetComponent{T}()"/>.
/// </remarks>
public class OverlapBoxDetector : MonoBehaviour
{
    [Tooltip("The half-extents (half-size) of the detection box.")]
    [SerializeField] private Vector3 _boxDimensions;

    [Tooltip("The local offset from the Transform position where the box center is located.")]
    [SerializeField] private Vector3 _boxOffset;

    /// <summary>
    /// Checks if any colliders within the box volume contain a specific component type.
    /// </summary>
    /// <typeparam name="T">The type of component to look for on the detected colliders.</typeparam>
    /// <param name="layerMask">The <see cref="LayerMask"/> used to filter which layers are checked.</param>
    /// <returns>True if at least one object of type <typeparamref name="T"/> is found.</returns>
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

    /// <summary>
    /// Checks for the presence of any colliders on the specified layers within the box volume.
    /// </summary>
    /// <param name="layerMask">The layers to include in the detection.</param>
    /// <returns>True if any collider is detected.</returns>
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

    /// <summary>
    /// Retrieves an array of all <see cref="Collider"/> components found within the box volume.
    /// </summary>
    /// <param name="layerMask">The layers to filter the search.</param>
    /// <returns>An array of <see cref="Collider"/> objects, or null if no objects are found.</returns>
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

    /// <summary>
    /// Draws a wireframe cube in the Scene view to visualize the detection volume.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            transform.position + _boxOffset,
            _boxDimensions
        );
    }
}