using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(OverlapSphereDetector))]
public abstract class LingeringDamageInflector : MonoBehaviour
{
    [SerializeField] protected float _duration;
    [SerializeField] protected float _damagePerTick;
    [SerializeField] protected float _timeAfterTick;
    [SerializeField] protected LayerMask _damageableLayers;
    [SerializeField] protected UnityEvent _onStart;
    [SerializeField] protected UnityEvent _onEnd;
    protected OverlapSphereDetector _sphereDetector;
    protected bool _hasBeenInitiatedAlready = false;

    private void Awake()
    {
        _sphereDetector = GetComponent<OverlapSphereDetector>();    
    }

    protected abstract IEnumerator LingeringLifeCycle();
    protected abstract void InflictDamage();
    public abstract void Init();
}