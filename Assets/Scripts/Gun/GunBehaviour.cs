using System;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
public class GunBehaviour : MonoBehaviour
{
    protected RayCastDetector _rayCastDetector;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
    }

    public virtual void Shoot(Vector3 origin, Vector3 dir, float range, float dmg)
    {
        Debug.Log("[GUN BEHAVIOUR] Shot fired -");

        RaycastHit hit;

        _rayCastDetector.Check(
            origin,
            dir,
            out hit,
            range
        );

        Debug.DrawRay(
            origin,
            dir * range,
            hit.collider == null ? Color.red : Color.green,
            3.0f
        );

        if (hit.collider == null)
            return;

        IShootable shootable = hit.collider.gameObject.GetComponent<IShootable>();

        if (shootable == null)
            return;

        shootable.Damage(dmg);
        
        Debug.Log($"[GUN BEHAVIOUR] Gun '{gameObject.name}' hit a target: {hit.collider.gameObject.name} -");

    }

    public virtual void Reload()
    {
        Debug.Log("[GUN BEHAVIOUR] Gun reloaded -");
    }
}