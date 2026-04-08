using UnityEngine;

public class WireCubeTransformGizmos : MonoBehaviour
{
    [SerializeField] private float _size;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            transform.position,
            Vector3.one * _size
        );
    }
}
