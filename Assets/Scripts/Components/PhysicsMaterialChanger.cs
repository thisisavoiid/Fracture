using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PhysicsMaterialChanger : MonoBehaviour
{
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void SetPhysicsMaterial(PhysicsMaterial material)
    {
        if (_collider.material = material)
            return;
        
        _collider.material = material;
    }
}