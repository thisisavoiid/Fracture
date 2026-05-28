using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class FlickeringLightConfig
{
    [SerializeField] private Range _flickerIntensity;
    public Range FlickerIntensity => _flickerIntensity;

    [SerializeField] private Range _intervals;
    public Range Intervals => _intervals;
}