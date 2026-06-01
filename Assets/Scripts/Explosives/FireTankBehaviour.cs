using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Explosives/Behaviours/Fire Tank Behaviour", fileName = "Fire Tank Behaviour")]
public class FireTankBehaviour : ExplosionBehaviour
{
    [SerializeField] private LingeringDamageInflector _damageInflector;

    public override void Explode(ExplosionContext ctx)
    {
        LingeringDamageInflector damageInflector = Instantiate(_damageInflector, ctx.Transform.position, Quaternion.identity);
        damageInflector.gameObject.name = _damageInflector.gameObject.name;
        damageInflector.Init();

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