using UnityEngine;

public class TargetTest : MonoBehaviour
{
    [SerializeField] private TransformVariable _transform;

    private void Awake()
    {
        _transform.SetValue(this.transform);
    }
}
