using UnityEngine;

public class SemiAutomaticBehaviour : GunBehaviour
{
    public override void Reload()
    {
        base.Reload();
    }

    public override void Shoot(Vector3 origin, Vector3 dir, float range, float dmg)
    {
        // Debug.Log($"[SEMI AUTOMATIC BEHAVIOUR] Semi-automatic gun '{gameObject.name}' fired a shot -");
        base.Shoot(origin, dir, range, dmg);
    }
}