using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(OverlapSphereDetector))]
public class ExplosiveController : MonoBehaviour
{
    [SerializeField] private ExplosionBehaviour _behaviour;
    [SerializeField] private Explosive _explosive;
    [SerializeField] private bool _addDynamicDetonationTime;
    private OverlapSphereDetector _sphereDetector;
    private bool _isAlreadyExploded = false;
    public UnityEvent OnExplode;
    public UnityEvent OnDetonationCycleStart;


    private void Awake()
    {
        _sphereDetector = GetComponent<OverlapSphereDetector>();
    }

    // private void OnEnable()
    // {
    //     if (_explosive.ExplodeSound != null && AudioManager.Instance != null)
    //         OnExplode.AddListener(() => AudioManager.Instance.PlaySound(_explosive.ExplodeSound, transform.position));

    //     if (_explosive.DetonationCycleStartSound != null && AudioManager.Instance != null)
    //         OnDetonationCycleStart.AddListener(() => AudioManager.Instance.PlaySound(_explosive.DetonationCycleStartSound, transform.position));
    // }

    private void OnDisable()
    {
        OnExplode.RemoveAllListeners();
        OnDetonationCycleStart.RemoveAllListeners();
    }

    [ContextMenu("Trigger Explosion")]
    public void TriggerExplosion()
    {
        if (_isAlreadyExploded)
            return;

        ExplosionContext ctx = new ExplosionContext
        {
            Explosive = _explosive,
            Transform = transform,
            OverlapSphereDetector = _sphereDetector,
            GameObject = this.gameObject
        };

        _sphereDetector.SetRadius(ctx.Explosive.Config.Radius);

        StartCoroutine(DetonationCycle(ctx));

        _isAlreadyExploded = true;
    }

    private IEnumerator DetonationCycle(ExplosionContext ctx)
    {
        OnDetonationCycleStart?.Invoke();

        float timeToWait = ctx.Explosive.Config.DetonationTime;

        if (_addDynamicDetonationTime && ctx.Explosive.DetonationCycleStartSound != null)
            timeToWait = ctx.Explosive.DetonationCycleStartSound.Config.Clip.length + timeToWait;

        Debug.Log($"[EXPLOSIVE CONTROLLER] Starting detonation cycle for {ctx.Explosive.name}. Delay: {timeToWait}s -");

        yield return new WaitForSeconds(timeToWait);

        _behaviour.Explode(ctx);

        OnExplode?.Invoke();

        Debug.Log($"[EXPLOSIVE CONTROLLER] Exploding {ctx.GameObject.name} at {ctx.Transform.position} -");

        // ctx.GameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}