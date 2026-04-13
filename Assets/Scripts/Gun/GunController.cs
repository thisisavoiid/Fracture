using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(GunBehaviourLoader))]
[RequireComponent(typeof(GunBulletTracker))]
public class GunController : Weapon
{
    [SerializeField] private Gun _gun;
    [SerializeField] private WeaponRecoil _weaponRecoilObject;
    private GunConfig _gunStats;
    private Transform _transform;
    private RayCastDetector _rayCastDetector;
    private GunBehaviour _gunBehaviour;
    private GunBehaviourLoader _gunBehaviourLoader;
    private GunBulletTracker _gunBulletTracker;

    private double _timePassedSinceLastShot;

    private void Awake()
    {
        _gunStats = _gun.Stats;
        _rayCastDetector = GetComponent<RayCastDetector>();
        _transform = GetComponent<Transform>();
        _gunBulletTracker = GetComponent<GunBulletTracker>();

        Debug.Log($"[GUN CONTROLLER] Loading behaviour for {_gun.Name} -");

        _gunBehaviourLoader = GetComponent<GunBehaviourLoader>();
        _gunBehaviourLoader.LoadBehaviour(_gun.Type, out _gunBehaviour);

        if (_gunBehaviour == null)
        {
            Debug.LogError($"[GUN CONTROLLER] An error occured while loading gun behaviour of {_gun.Name} -");
            return;
        }

        Debug.Log($"[GUN CONTROLLER] Set behaviour for {_gun.Name}: {_gunBehaviour.GetType().Name}-");

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gunStats.ToString()} -");

        _timePassedSinceLastShot = 60 / _gunStats.ShotsPerMinute;

        _gunBulletTracker.LoadGunStats(_gun);
        _gunBulletTracker.ResetBullets();
    }

    private double CalculateDurationAfterShot(int shotsPerMinute) => 60.0d / (double)shotsPerMinute;
    private void ResetBulletTimer() => _timePassedSinceLastShot = 0.0f;

    public void HandleShooting(Vector3 origin, Vector3 dir, bool isHeldDown, bool wasPressedThisFrame)
    {
        double durationAfterShot = CalculateDurationAfterShot(_gunStats.ShotsPerMinute);

        if (_timePassedSinceLastShot < durationAfterShot)
            return;

        if (!_gunBulletTracker.HasBulletsLeft())
        {
            Debug.Log($"[GUN CONTROLLER] Couldn't decrease remaining bullets count on {_gun.name} because the gun doesn't have any bullets left -");
            return;
        }
            
        switch (_gun.Type)
        {
            case GunType.Semi_Automatic:
                if (!wasPressedThisFrame)
                    return;

                Shoot(origin, dir);
                ResetBulletTimer();
                break;

            case GunType.Automatic:
                Shoot(origin, dir);
                ResetBulletTimer();

                break;
        }
    }

    private void Update()
    {
        _timePassedSinceLastShot += Time.deltaTime;
    }

    private void Shoot(Vector3 origin, Vector3 dir)
    {
        if (_gunBehaviour == null)
        {
            Debug.LogError("[GUN CONTROLLER] Couldn't execute shot because the specified gun behaviour doesn't exist -");
            return;
        }

        Debug.Log($"[GUN CONTROLLER] Executing shot for gun: {_gun.Type} -");
        
        
        _gunBehaviour.Shoot(
            origin,
            dir,
            _gunStats.Range,
            _gunStats.DamagePerShot
        );

        _gunBulletTracker.DecreaseRemainingBulletCount();

        if (_gun.Sound == null || _gun.Sound?.Data.Clip == null)
            return;

        AudioManager.Instance.PlaySound(_gun.Sound);
        _weaponRecoilObject?.ApplyRecoil();
    }

    public override void Use(Vector3 origin, Vector3 dir, bool held, bool pressed)
    {
        HandleShooting(
            origin,
            dir,
            held,
            pressed
        );
    }

    public override void Reload()
    {
        _gunBulletTracker.ResetBullets();
    }
}