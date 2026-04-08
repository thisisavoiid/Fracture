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

        return Physics.Raycast(
            ray,
            range,
            _layerMask
        );
    }

    public void Check(Vector3 origin, Vector3 dir, out RaycastHit hit, float range=Mathf.Infinity)
    {
        Ray ray = BuildRay(origin, dir);

        Physics.Raycast(
            ray,
            out hit,
            range,
            _layerMask
        );
    }
}