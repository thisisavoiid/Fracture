using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(GunBulletTracker))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(DecalSpawner))]
public class GunController : Weapon
{
    [SerializeField] private Transform _projectileSpawnTransform;
    [SerializeField] private GunContextEvent _onGunShotEvent;

    private GunConfig _gun => Config as GunConfig;
    private RayCastDetector _rayCastDetector;
    private GunBulletTracker _gunBulletTracker;
    private DecalSpawner _decalSpawner;
    private Timer _timer;

    public UnityEvent<GunConfig> OnShoot;
    public UnityEvent<GunConfig> OnReload;
    public UnityEvent<GunConfig> OnGunInitialized;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
        _gunBulletTracker = GetComponent<GunBulletTracker>();
        _timer = GetComponent<Timer>();
        _decalSpawner = GetComponent<DecalSpawner>();

        _timer.SetTime(new TimeMS(CalculateDurationAfterShot(_gun.Stats.ShotsPerMinute)));
        _timer.Start();

        SetHolder(transform.root.gameObject);

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gun.Stats.ToString()} -");
    }

    private void Start()
    {
        OnGunInitialized?.Invoke(_gun);

        if (_gun.ShootSound != null && AudioManager.Instance != null)
            OnShoot.AddListener((_gun) => AudioManager.Instance.PlaySound(_gun.ShootSound, transform.position));

        if (_gun.ReloadSound != null && AudioManager.Instance != null)
            OnReload.AddListener((_gun) => AudioManager.Instance.PlaySound(_gun.ReloadSound, transform.position));
    }

    private float CalculateDurationAfterShot(int shotsPerMinute) => 60.0f / (float)shotsPerMinute;

    public override void Use(Vector3 origin, Vector3 dir, bool held, bool pressed)
    {
        GunContext gunContext = new GunContext()
        {
            Holder = _holder,
            Gun = _gun,
            Direction = dir.normalized,
            Origin = origin,
            RayCastDetector = _rayCastDetector,
            BulletTracker = _gunBulletTracker,
            IsHeld = held,
            IsPressed = pressed,
            Timer = _timer,
            ProjectileSpawnTransform = _projectileSpawnTransform
        };

        bool wasShotSuccessful = _gun.Behaviour.Shoot(gunContext, out RaycastHit hit);

        if (!wasShotSuccessful)
            return;

        OnShoot?.Invoke(_gun);
        _onGunShotEvent.Invoke(gunContext);
        
        if (hit.collider == null)
            return;

        Debug.Log($"[GUN CONTROLLER] Shot object: {hit.collider.gameObject.name} at point: {hit.point.ToString()} -");
        _decalSpawner.SpawnDecal(hit.point, Quaternion.LookRotation(-hit.normal), hit.collider.gameObject.transform);

    }

    public override void Reload()
    {
        if (_gunBulletTracker.BulletsRemaining >= _gun.Stats.TotalRounds)
            return;

        OnReload?.Invoke(_gun);
    }
}