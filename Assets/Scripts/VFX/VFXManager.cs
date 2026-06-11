using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    private static VFXManager _instance;
    public static VFXManager Instance => _instance;

    [SerializeField]
    [Required]
    private VFXEmitter _emitterPrefab;

    [SerializeField] private List<VFXPoolObject> _vfxPoolTargets = new();
    private Dictionary<VFXType, List<VFXEmitter>> _vfxEmitterInstances = new();
    private List<VFXEmitter> _activeVFXEmitters = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < _vfxPoolTargets.Count; i++)
        {
            VFXPoolObject poolObject = _vfxPoolTargets[i];

            if (!_vfxEmitterInstances.ContainsKey(poolObject.VFXType))
                _vfxEmitterInstances.Add(poolObject.VFXType, new List<VFXEmitter>());

            for (int j = 0; j < poolObject.PoolSize; j++)
            {
                VFXEmitter vfxEmitter = Instantiate(_emitterPrefab, transform);
                vfxEmitter.name = $"VFX Emitter ({poolObject.EffectAsset.name}) {j+1:D2}";
                vfxEmitter.SetEffect(poolObject.EffectAsset);
                vfxEmitter.gameObject.SetActive(false);

                _vfxEmitterInstances[poolObject.VFXType].Add(vfxEmitter);
            }
        }
    }

    private VFXEmitter GetVacantVFXEmitter(VFXType type)
    {
        if (!_vfxEmitterInstances.ContainsKey(type))
        {
            Debug.LogError($"[VFX CONTROLLER] The VFX type {type} has not been initiated properly! -");
            return null;
        }

        foreach (VFXEmitter vfxEmitter in _vfxEmitterInstances[type])
        {
            if (!vfxEmitter.gameObject.activeInHierarchy)
                return vfxEmitter;
        }

        return null;
    }

    public void PlayVFX(VFXType type, Vector3 pos)
    {
        VFXEmitter vfxEmitter = GetVacantVFXEmitter(type);

        if (vfxEmitter == null)
            return;
        
        vfxEmitter.SetPosition(pos);
        vfxEmitter.gameObject.SetActive(true);
        vfxEmitter.PlayEffect();
        _activeVFXEmitters.Add(vfxEmitter);
    }

    private void Update()
    {
        if (_activeVFXEmitters == null || _activeVFXEmitters.Count == 0)
            return;
        
        List<VFXEmitter> activeVFXEmitters = new();

        foreach (VFXEmitter vfxEmitter in _activeVFXEmitters)
            activeVFXEmitters.Add(vfxEmitter);
        
        foreach (VFXEmitter vfxEmitter in activeVFXEmitters)
        {
            if (vfxEmitter == null)
            {
                _activeVFXEmitters.Remove(vfxEmitter);
                continue;
            }
            
            if (!vfxEmitter.gameObject.activeInHierarchy)
            {
                _activeVFXEmitters.Remove(vfxEmitter);
                continue;
            }

            if (vfxEmitter.GetParticleCount() == 0)
            {
                vfxEmitter.StopEffect();
                _activeVFXEmitters.Remove(vfxEmitter);
            }
        }
    }
}
