using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(GunBulletTracker))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(DecalSpawner))]
public class GunController : Weapon
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _projectileSpawnTransform;
    private GunConfig _gunStats;
    private RayCastDetector _rayCastDetector;
    private GunBulletTracker _gunBulletTracker;
    private DecalSpawner _decalSpawner;
    private Timer _timer;
    private Transform _transform;

    public UnityEvent<Gun> OnShoot;
    public UnityEvent<Gun> OnReload;
    public UnityEvent<Gun> OnGunInitialized;

    private void Awake()
    {
        _rayCastDetector = GetComponent<RayCastDetector>();
        _gunBulletTracker = GetComponent<GunBulletTracker>();
        _timer = GetComponent<Timer>();
        _decalSpawner = GetComponent<DecalSpawner>();

        _timer.SetTime(CalculateDurationAfterShot(_gun.Stats.ShotsPerMinute));
        _timer.Start();

        _transform = GetComponent<Transform>();

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gun.Stats.ToString()} -");
    }

    private void OnEnable()
    {
        if (_gun.ShootSound != null)
            OnShoot.AddListener((_gun) => AudioManager.Instance.PlaySound(_gun.ShootSound, transform.position));

        if (_gun.ReloadSound != null)
            OnReload.AddListener((_gun) => AudioManager.Instance.PlaySound(_gun.ReloadSound, transform.position));
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

        if (_gun.Projectile != null)
        {
            var projectile = Instantiate(_gun.Projectile, _projectileSpawnTransform.position, Quaternion.identity);

            if (hit.collider == null)
                projectile.Init(dir);
            else
                projectile.Init((hit.point - _projectileSpawnTransform.position).normalized);
        }

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