using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(DecalProjector))]
public class DecalObject : MonoBehaviour
{
    private DecalProjector _projector;
    private bool _hasBeenInitialized = false;

    private void Awake()
    {
        _projector = GetComponent<DecalProjector>();
    }

    public void Initialize(DecalConfig config)
    {
        if (_hasBeenInitialized)
            return;

        if (config.Material == null)
            return;

        _projector.material = config.Material;
        
        if (config.StayVisibleForever)
            return;

        if (config.DestroyAfter <= 0)
            return;

        float randomRange = Random.Range(-config.RandomRange, config.RandomRange);

        _projector.size = new Vector3(
            config.DefaultSize + randomRange, 
            config.DefaultSize + randomRange, 
            config.DefaultSize + randomRange
        );

        StartCoroutine(DecalObjectLifeCycle(config.DestroyAfter));
    }

    private IEnumerator DecalObjectLifeCycle(float destroyAfter)
    {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(this.gameObject);
    }
}
