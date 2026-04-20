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

        if (!CanShoot(gunCtx.IsPressed, gunCtx.IsHeld, gunCtx.BulletTracker.HasBulletsLeft(), gunCtx.Timer.GetRemainingTime()))
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

        if (hit.collider == null)
            return true;

        IShootable shootable = hit.collider.gameObject.GetComponent<IShootable>();

        if (shootable == null)
            return true;

        shootable.Hit(gunCtx.Gun.Stats.DamagePerShot, hit.point);

        return true;
    }
}