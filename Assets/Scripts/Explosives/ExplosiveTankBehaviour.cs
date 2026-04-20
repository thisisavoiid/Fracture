using UnityEngine;

[CreateAssetMenu(menuName = "Explosives/Behaviours/Explosive Tank Behaviour", fileName = "Explosive Tank Behaviour")]
public class ExplosiveTankBehaviour : ExplosionBehaviour
{
    public override void Explode(ExplosionContext ctx)
    {
        Collider[] colliders = ctx.OverlapSphereDetector.GetColliders(ctx.Explosive.Config.TargetLayers);

        foreach (var obj in colliders)
        {
            IShootable shootable = obj.GetComponent<IShootable>();

            if (shootable != null && obj.gameObject != ctx.GameObject)
            {
                shootable.Hit(ctx.Explosive.Config.Damage);
            }
        }
    }
}