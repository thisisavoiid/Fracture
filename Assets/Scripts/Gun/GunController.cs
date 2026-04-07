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


    private void Awake()
    {
        _gunStats = _gun.Stats;
        _rayCastDetector = GetComponent<RayCastDetector>();
        _transform = GetComponent<Transform>();

        Debug.Log($"[GUN CONTROLLER] Loading behaviour for {_gun.Name} -");

        _gunBehaviourLoader = GetComponent<GunBehaviourLoader>();
        _gunBehaviourLoader.LoadBehaviour(_gun.Type, out _gunBehaviour);

        Debug.Log($"[GUN CONTROLLER] Set behaviour for {_gun.Name}: {_gunBehaviour.GetType().Name}-");

        Debug.Log($"[GUN CONTROLLER] Initialized gun with the following configuration: \n{_gunStats.ToString()} -");
    }

    public void Shoot(Vector3 origin, Vector3 dir)
    {
        _gunBehaviour.Shoot(
            origin,
            dir,
            _gunStats.Range,
            _gunStats.DamagePerShot
        );
    }
}