using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
public class VFXPoolObject
{
    [SerializeField] private VisualEffectAsset _effectAsset;
    public VisualEffectAsset EffectAsset => _effectAsset;

    [SerializeField] private VFXType _vfxType;
    public VFXType VFXType => _vfxType;

    [SerializeField] private int _poolSize;
    public int PoolSize => _poolSize;

}
