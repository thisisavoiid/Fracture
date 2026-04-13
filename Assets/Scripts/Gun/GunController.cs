using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(GunBulletTracker))]
[RequireComponent(typeof(Timer))]
public class GunController : Weapon
{
    [SerializeField] private Gun _gun;
    [SerializeField] private WeaponRecoil _weaponRecoilObject;
    private GunConfig _gunStats;
    private RayCastDetector _rayCastDetector;
    private GunBulletTracker _gunBulletTracker;
    private AudioManager _audioManager;
    private Timer _timer;

    public UnityEvent<Gun> OnShoot;
    public UnityEvent<Gun> OnReload;
    public UnityEvent<Gun> OnGunInitialized;

    private float _timePassedSinceLastShot;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
        _gunBulletTracker = GetComponent<GunBulletTracker>();
        _audioManager = GetComponent<AudioManager>();
        _timer = GetComponent<Timer>();

        _timer.SetTime(CalculateDurationAfterShot(_gun.Stats.ShotsPerMinute));
        _timer.Start();

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gunStats.ToString()} -");
    }

    private void OnEnable()
    {
        OnShoot.AddListener((_gun) => _audioManager.PlaySound(_gun.Sound));
    }

    private void OnDisable()
    {
        OnShoot.RemoveAllListeners();
    }

    private void Start()
    {
        OnGunInitialized?.Invoke(_gun);
    }

    private float CalculateDurationAfterShot(int shotsPerMinute) => 60.0f / (float)shotsPerMinute;

    // private void ResetBulletTimer() => _timePassedSinceLastShot = 0.0f;

    // public void HandleShooting(Vector3 origin, Vector3 dir, bool isHeldDown, bool wasPressedThisFrame)
    // {
    //     double durationAfterShot = CalculateDurationAfterShot(_gunStats.ShotsPerMinute);

    //     if (_timePassedSinceLastShot < durationAfterShot)
    //         return;

    //     if (!_gunBulletTracker.HasBulletsLeft())
    //     {
    //         Debug.Log($"[GUN CONTROLLER] Couldn't decrease remaining bullets count on {_gun.name} because the gun doesn't have any bullets left -");
    //         return;
    //     }

    //     switch (_gun.Type)
    //     {
    //         case GunType.Semi_Automatic:
    //             if (!wasPressedThisFrame)
    //                 return;

    //             Shoot(origin, dir);
    //             ResetBulletTimer();
    //             break;

    //         case GunType.Automatic:
    //             Shoot(origin, dir);
    //             ResetBulletTimer();

    //             break;
    //     }
    // }

    private void Update()
    {
        _timePassedSinceLastShot += Time.deltaTime;
    }

    public override void Use(Vector3 origin, Vector3 dir, bool held, bool pressed)
    {
        GunContext gunContext = new GunContext()
        {
            Gun = _gun,
            Direction = dir,
            Origin = origin,
            RayCastDetector = _rayCastDetector,
            BulletTracker = _gunBulletTracker,
            IsHeld = held,
            IsPressed = pressed,
            Timer = _timer
        };

        bool shotSuccess = _gun.Behaviour.Shoot(gunContext);

        if (!shotSuccess)
            return;

        OnShoot?.Invoke(_gun);
    }

    public override void Reload() => OnReload?.Invoke(_gun);
}