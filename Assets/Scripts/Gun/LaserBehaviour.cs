using UnityEngine;

/// <summary>
/// Implements a laser-style firing behavior for weapons, handling hit detection via raycasting 
/// and projectile instantiation for visual or physical effects.
/// </summary>
[CreateAssetMenu(menuName = "Gun/Behaviours/LaserBehaviour")]
public class LaserBehaviour : GunBehaviour
{
    /// <summary>
    /// Determines if the weapon is in a state where it can fire based on input and cooldown.
    /// </summary>
    private bool CanShoot(bool isHeld, float remainingTime) => isHeld && remainingTime <= 0;

    public override bool Shoot(GunContext gunCtx, out RaycastHit hit)
    {
        hit = new RaycastHit();

        // Validate cooldown and hold status
        if (!CanShoot(gunCtx.IsHeld, gunCtx.Timer.GetRemainingTime().TotalSeconds))
            return false;

        gunCtx.Timer.Reset();

        // Perform the hit detection
        gunCtx.RayCastDetector.Check(
            gunCtx.Origin,
            gunCtx.Direction,
            out hit,
            gunCtx.Gun.Stats.Range
        );

        // Visual debug aid in the Scene view
        Debug.DrawRay(
            gunCtx.Origin,
            gunCtx.Direction * gunCtx.Gun.Stats.Range,
            hit.collider == null ? Color.red : Color.green,
            3.0f
        );

        // Handle visual projectile spawning
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

        // Apply damage if the target is shootable
        if (hit.collider.gameObject.TryGetComponent(out IShootable shootable))
            shootable.Hit(gunCtx.Gun.Stats.DamagePerShot, hit.point);

        return true;
    }
}