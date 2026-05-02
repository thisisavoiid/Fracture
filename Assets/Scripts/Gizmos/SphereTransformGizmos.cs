using UnityEngine;

public class SphereTransformGizmos : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private Color _color;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(_color.r, _color.g, _color.b);

        Gizmos.DrawSphere(
            transform.position,
            _radius
        );
    }
}
