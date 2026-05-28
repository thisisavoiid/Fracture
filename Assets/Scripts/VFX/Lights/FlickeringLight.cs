using NaughtyAttributes;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private Light _lightSource;
    [SerializeField] private FlickeringLightConfig _config;

    [SerializeField]
    [ReadOnly]
    private float _timeDelta = 0.0f;

    [SerializeField]
    [ReadOnly]
    private float _targetTime = 0.0f;

    private void Awake()
    {
        if (_lightSource != null)
            return;

        _lightSource = GetComponent<Light>();
    }

    private void Update()
    {
        _timeDelta += Time.deltaTime;

        if (_timeDelta >= _targetTime)
        {
            _timeDelta = 0.0f;
            RefreshLightIntensity();
            _targetTime = GetRandomInterval();
        }
    }

    private void RefreshLightIntensity()
    {
        float intensity = Random.Range(_config.FlickerIntensity.Min, _config.FlickerIntensity.Max);
        _lightSource.intensity = intensity;
    }

    private float GetRandomInterval() => Random.Range(_config.Intervals.Min, _config.Intervals.Max);
}
