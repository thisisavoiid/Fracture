using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RayCastDetector))]
[RequireComponent(typeof(GunBehaviourLoader))]
public class GunController : MonoBehaviour
{
    [SerializeField] private Gun _gun;
    private GunConfig _gunStats;
    private Transform _transform;
    private RayCastDetector _rayCastDetector;
    private GunBehaviour _gunBehaviour;
    private GunBehaviourLoader _gunBehaviourLoader;

    private double _timePassedSinceLastShot;

    private void Awake()
    {
        _gunStats = _gun.Stats;
        _rayCastDetector = GetComponent<RayCastDetector>();
        _transform = GetComponent<Transform>();

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
    }

    private double CalculateDurationAfterShot(int shotsPerMinute) => 60.0d / (double)shotsPerMinute;
    private void ResetBulletTimer() => _timePassedSinceLastShot = 0.0f;

    public void HandleShooting(Vector3 origin, Vector3 dir, bool isHeldDown, bool wasPressedThisFrame)
    {
        double durationAfterShot = CalculateDurationAfterShot(_gunStats.ShotsPerMinute);

        if (_timePassedSinceLastShot < durationAfterShot)
            return;

        switch (_gun.Type)
        {
            case GunType.Semi_Automatic:
                if (!wasPressedThisFrame)
                    return;

                Shoot(origin, dir);
                ResetBulletTimer();
                break;

            case GunType.Automatic:
                if (wasPressedThisFrame)
                    ResetBulletTimer();

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
    }
}