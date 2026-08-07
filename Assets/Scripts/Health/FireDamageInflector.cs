using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FireDamageInflector : LingeringDamageInflector
{
    private const int MaxColliders = 16;
    private Collider[] _colliderCache;
    private Dictionary<GameObject, IShootable> _shootableCache = new(MaxColliders);

    [ContextMenu("Force Init")]
    public override void Init()
    {
        if (_hasBeenInitiatedAlready)
            return;

        _hasBeenInitiatedAlready = true;
        StartCoroutine(LingeringLifeCycle());
        _onStart?.Invoke();
    }

    protected override IEnumerator LingeringLifeCycle()
    {
        float totalTimePassed = 0.0f;
        float tickTimePassed = 0.0f;

        while (totalTimePassed < _duration)
        {
            if (tickTimePassed >= _timeAfterTick)
            {
                InflictDamage();
                tickTimePassed = 0.0f;
            }

            totalTimePassed += Time.deltaTime;
            tickTimePassed += Time.deltaTime;

            yield return null;
        }

        _onEnd?.Invoke();

        Destroy(this.gameObject);
    }
    
    protected override void InflictDamage()
    {
        _colliderCache = new Collider[MaxColliders];
        
        int colliderCount = _sphereDetector.GetCollidersNonAlloc(_damageableLayers, _colliderCache);
        
        if (colliderCount == 0)
            return;

        _shootableCache.Clear();

        for (int i=0; i<colliderCount; i++)
        {
            Collider collider = _colliderCache[i];
            
            if (collider == null) continue;

            GameObject go = collider.gameObject;
            
            Debug.Log(go.name);

            if (!_shootableCache.TryGetValue(go, out IShootable shootable))
            {
                if (!go.TryGetComponent(out shootable))
                    continue;
                
                _shootableCache.Add(go, shootable);
            }

            shootable.Hit(_damagePerTick, transform.position);
        }
    }
}
