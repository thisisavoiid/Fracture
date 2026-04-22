using UnityEngine;

public class RayCastDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private Ray BuildRay(Vector3 origin, Vector3 dir)
    {
        return new Ray(
            origin,
            dir
        );
    }

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

    public bool Check(Vector3 origin, Vector3 dir, out RaycastHit hit, LayerMask layerMask, float range=Mathf.Infinity)
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