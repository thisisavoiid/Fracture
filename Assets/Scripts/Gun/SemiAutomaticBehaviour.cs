using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Behaviours/SemiAutomatic")]
public class SemiAutomaticBehaviour : GunBehaviour
{
    private bool CanShoot(
        bool isPressed,
        bool isHeld,
        bool bulletsLeft,
        float remainingTime
    ) => isPressed && bulletsLeft && remainingTime <= 0.0f;

    public override bool Shoot(GunContext gunCtx, out RaycastHit hit)
    {
        hit = new RaycastHit();

        if (!CanShoot(gunCtx.IsPressed, gunCtx.IsHeld, gunCtx.BulletTracker.HasBulletsLeft(), gunCtx.Timer.GetRemainingTime().TotalSeconds))
            return false;

        gunCtx.Timer.Reset();

        gunCtx.RayCastDetector.Check(
            gunCtx.Origin,
            gunCtx.Direction,
            out hit,
            gunCtx.Gun.Stats.Range
        );

        Debug.DrawRay(
            gunCtx.Origin,
            gunCtx.Direction * gunCtx.Gun.Stats.Range,
            hit.collider == null ? Color.red : Color.green,
            3.0f
        );

        if (gunCtx.Gun.Projectile != null)
        {
            var projectile = Instantiate(gunCtx.Gun.Projectile, gunCtx.ProjectileSpawnTransform.position, Quaternion.identity);

            if (hit.collider == null)
            {
                projectile.Init(gunCtx.Direction, gunCtx.Gun.Stats.DamagePerShot);
            }
            else
            {
                projectile.Init(
                    (hit.point - gunCtx.ProjectileSpawnTransform.position).normalized,
                    (hit.point - gunCtx.Origin).magnitude
                );
            }
        }

        if (hit.collider == null)
            return true;

        if (hit.collider.gameObject.TryGetComponent(out IShootable shootable))
            shootable.Hit(gunCtx.Gun.Stats.DamagePerShot, hit.point);

        return true;

    }
}