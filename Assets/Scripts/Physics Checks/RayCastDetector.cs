using UnityEngine;

/// <summary>
/// Provides utility methods for performing raycast detection with integrated debug visualization.
/// </summary>
public class RayCastDetector : MonoBehaviour
{
    [Tooltip("The default layer mask used for raycast detection.")]
    [SerializeField] private LayerMask _layerMask;

    /// <summary>
    /// Constructs a <see cref="Ray"/> object using an origin and direction.
    /// </summary>
    private Ray BuildRay(Vector3 origin, Vector3 dir)
    {
        return new Ray(
            origin,
            dir
        );
    }

    /// <summary>
    /// Checks for a collision along a ray and draws a debug line.
    /// </summary>
    /// <param name="origin">The starting point of the ray.</param>
    /// <param name="dir">The direction of the ray.</param>
    /// <param name="range">The maximum distance the ray should check.</param>
    /// <returns>True if the ray intersects a collider within the <see cref="_layerMask"/>.</returns>
    public bool Check(Vector3 origin, Vector3 dir, float range)
    {
        Ray ray = BuildRay(origin, dir);

        bool hasHit = Physics.Raycast(
            ray,
            range,
            _layerMask
        );

        Debug.DrawLine(
            origin,
            origin + dir.normalized * range,
            hasHit ? Color.green : Color.red
        );

        return hasHit;
    }

    /// <summary>
    /// Checks for a collision and outputs <see cref="RaycastHit"/> information.
    /// </summary>
    /// <param name="origin">The starting point of the ray.</param>
    /// <param name="dir">The direction of the ray.</param>
    /// <param name="hit">Output parameter containing detailed collision data.</param>
    /// <param name="range">The maximum distance of the ray (defaults to infinity).</param>
    /// <returns>True if a collision occurs.</returns>
    public bool Check(Vector3 origin, Vector3 dir, out RaycastHit hit, float range = Mathf.Infinity)
    {
        Ray ray = BuildRay(origin, dir);

        bool hasHit = Physics.Raycast(
            ray,
            out hit,
            range,
            _layerMask
        );

        float drawRange = hasHit ? hit.distance : range;

        Debug.DrawLine(
            origin,
            origin + dir.normalized * drawRange,
            hasHit ? Color.green : Color.red
        );

        return hasHit;
    }

    /// <summary>
    /// Checks for a collision using a specific <see cref="LayerMask"/> override.
    /// </summary>
    /// <param name="origin">The starting point of the ray.</param>
    /// <param name="dir">The direction of the ray.</param>
    /// <param name="hit">Output parameter containing detailed collision data.</param>
    /// <param name="layerMask">The specific layers to include in this detection check.</param>
    /// <param name="range">The maximum distance of the ray.</param>
    /// <returns>True if a collision occurs on the specified layers.</returns>
    public bool Check(Vector3 origin, Vector3 dir, out RaycastHit hit, LayerMask layerMask, float range = Mathf.Infinity)
    {
        Ray ray = BuildRay(origin, dir);

        bool hasHit = Physics.Raycast(
            ray,
            out hit,
            range,
            layerMask
        );

        float drawRange = hasHit ? hit.distance : range;

        Debug.DrawLine(
            origin,
            origin + dir.normalized * drawRange,
            hasHit ? Color.green : Color.red
        );

        return hasHit;
    }
}