using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(GunBulletTracker))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(DecalSpawner))]
public class GunController : Weapon
{
    [SerializeField] private Gun _gun;
    [SerializeField] private WeaponRecoil _weaponRecoilObject;
    private GunConfig _gunStats;
    private RayCastDetector _rayCastDetector;
    private GunBulletTracker _gunBulletTracker;
    private AudioManager _audioManager;
    private DecalSpawner _decalSpawner;
    private Timer _timer;

    public UnityEvent<Gun> OnShoot;
    public UnityEvent<Gun> OnReload;
    public UnityEvent<Gun> OnGunInitialized;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
        _gunBulletTracker = GetComponent<GunBulletTracker>();
        _timer = GetComponent<Timer>();
        _decalSpawner = GetComponent<DecalSpawner>();
        _audioManager = GetComponent<AudioManager>();

        _timer.SetTime(CalculateDurationAfterShot(_gun.Stats.ShotsPerMinute));
        _timer.Start();

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gun.Stats.ToString()} -");
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

    public override void Use(Vector3 origin, Vector3 dir, bool held, bool pressed)
    {
        GunContext gunContext = new GunContext()
        {
            Gun = _gun,
            Direction = dir.normalized,
            Origin = origin,
            RayCastDetector = _rayCastDetector,
            BulletTracker = _gunBulletTracker,
            IsHeld = held,
            IsPressed = pressed,
            Timer = _timer
        };

        bool wasShotSuccessful = _gun.Behaviour.Shoot(gunContext, out RaycastHit hit);

        if (!wasShotSuccessful)
            return;

        OnShoot?.Invoke(_gun);

        if (hit.collider == null)
            return;

        Debug.Log($"[GUN CONTROLLER] Shot object: {hit.collider.gameObject.name} at point: {hit.point.ToString()} -");
        _decalSpawner.SpawnDecal(hit.point, Quaternion.LookRotation(-hit.normal), hit.collider.gameObject.transform);
    }

    public override void Reload() => OnReload?.Invoke(_gun);
}