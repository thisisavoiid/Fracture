using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Explosives/Behaviours/Fire Tank Behaviour", fileName = "Fire Tank Behaviour")]
public class FireTankBehaviour : ExplosionBehaviour
{
    // objekt was fire damage inflected!
    [SerializeField] private LingeringDamageInflector _damageInflector;

    public override void Explode(ExplosionContext ctx)
    {
        LingeringDamageInflector damageInflector = Instantiate(_damageInflector, ctx.Transform.position, Quaternion.identity);
        damageInflector.gameObject.name = _damageInflector.gameObject.name;
        damageInflector.Init();

        List<Collider> colliders = ctx.OverlapSphereDetector.GetColliders(ctx.Explosive.Config.TargetLayers);

        foreach (var obj in colliders)
        {
            IShootable shootable = obj.GetComponent<IShootable>();

            if (shootable != null && obj.gameObject != ctx.GameObject)
            {
                shootable.Hit(ctx.Explosive.Config.Damage, ctx.GameObject.transform.position);
            }
        }
    }
}