using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FireDamageInflector : LingeringDamageInflector
{
    public override void Init()
    {
        Debug.Log("init!");
        if (_hasBeenInitiatedAlready)
            return;

        _hasBeenInitiatedAlready = true;
        StartCoroutine(LingeringLifeCycle());
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

        Destroy(this.gameObject);
    }

    protected override void InflictDamage()
    {
        List<Collider> damageColliders = _sphereDetector.GetColliders(_damageableLayers);

        if (damageColliders == null || damageColliders.Count == 0)
            return;

        foreach (Collider collider in damageColliders)
        {
            IShootable shootable = collider.gameObject.GetComponent<IShootable>();

            if (shootable == null)
                continue;

            shootable.Hit(_damagePerTick, transform.position);
        }
    }
}
