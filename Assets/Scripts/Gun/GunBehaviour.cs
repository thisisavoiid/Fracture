using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
public class GunBehaviour : MonoBehaviour
{
    protected RayCastDetector _rayCastDetector;
    protected LineRenderer _lineRenderer;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public virtual void Shoot(Vector3 origin, Vector3 dir, float range, float dmg)
    {
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

        shootable.Hit(dmg);
        
        Debug.Log($"[GUN BEHAVIOUR] Gun '{gameObject.name}' hit a target: {hit.collider.gameObject.name} -");

    }
}