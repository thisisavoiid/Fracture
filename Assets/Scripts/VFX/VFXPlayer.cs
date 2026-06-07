using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private bool _playOnAwake = false;

    [Button]
    public void PlayVFX()
    {
        _visualEffect.Play();
    }
    
    [Button]
    public void StopVFX()
    {
        _visualEffect.Stop();
    }

    private void Awake()
    {
        if (!_playOnAwake)
            _visualEffect.Stop();
    }
}