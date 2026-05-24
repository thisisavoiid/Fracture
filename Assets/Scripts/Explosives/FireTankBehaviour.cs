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
            if (!obj.TryGetComponent(out IShootable shootable))
                continue;

            if (obj.gameObject == ctx.GameObject)
                continue;
            
            shootable.Hit(ctx.Explosive.Config.Damage, ctx.Transform.position);
        }
    }
}