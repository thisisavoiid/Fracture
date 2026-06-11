using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXEmitter : MonoBehaviour
{
    private VisualEffect _visualEffect;

    private void Awake()
    {
        _visualEffect = GetComponent<VisualEffect>();
    }

    public void SetEffect(VisualEffectAsset effect)
    {
        _visualEffect.visualEffectAsset = effect;
    }

    public void SetPosition(Vector3 pos) => transform.position = pos;

    public void PlayEffect()
    {
        this.gameObject.SetActive(true);
        _visualEffect.Play();
    }

    public void StopEffect()
    {
        _visualEffect.Stop();
        this.gameObject.SetActive(false);
    }

    public int GetParticleCount()
    {
        if (_visualEffect == null)
            return 0;
        
        return _visualEffect.aliveParticleCount;
    }
}