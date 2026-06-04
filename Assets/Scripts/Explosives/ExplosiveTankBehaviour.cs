using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Explosives/Behaviours/Explosive Tank Behaviour", fileName = "Explosive Tank Behaviour")]
public class ExplosiveTankBehaviour : ExplosionBehaviour
{
    public override void Explode(ExplosionContext ctx)
    {
        List<Collider> colliders = ctx.OverlapSphereDetector.GetColliders(ctx.Explosive.Config.TargetLayers);

        foreach (var obj in colliders)
        {
            if (obj.gameObject == ctx.GameObject)
                continue;

            if (obj.TryGetComponent(out IShootable shootable))
                shootable.Hit(ctx.Explosive.Config.Damage, ctx.Transform.position);
            
            if (!obj.TryGetComponent(out Rigidbody rb))
                continue;

            rb.AddExplosionForce(
                ctx.Explosive.Config.ExplosionForce,
                ctx.Transform.position,
                ctx.Explosive.Config.Radius,
                ctx.Explosive.Config.UpwardsModifier,
                ctx.Explosive.Config.ForceMode
            );
        }
    }
}