using UnityEngine;

public class WireCubeTransformGizmos : MonoBehaviour
{
    [SerializeField] private Vector3 _dimensions;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Color _color;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(_color.r, _color.g, _color.b);

        Gizmos.DrawWireCube(
            transform.position + _offset,
            _dimensions
        );
    }
}
