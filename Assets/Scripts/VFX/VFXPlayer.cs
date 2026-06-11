using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private VFXType _vfxType;

    public void PlayEffect()
    {
        PlayEffect(transform.position);
    }

    public void PlayEffect(Vector3 pos)
    {
        VFXManager vfxManagerInstance = VFXManager.Instance;

        if (vfxManagerInstance == null)
            return;

        vfxManagerInstance.PlayVFX(_vfxType, pos);
    }
}
