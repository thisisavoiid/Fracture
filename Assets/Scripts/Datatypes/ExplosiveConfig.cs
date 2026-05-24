using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct ExplosiveConfig
{
    [Header("Explosion Metrics")]
    [Tooltip("The radius of the explosion's effect in meters.")]
    [Range(0.1f, 50.0f)]
    public float Radius;

    [Tooltip("The maximum damage dealt at the center of the explosion.")]
    [Range(0.0f, 1000.0f)]
    public float Damage;

    [Tooltip("The relative force applied upon explosion.")]
    [MinValue(0.0f)]
    public float ExplosionForce;

    [Tooltip("The weight of upwards motion upon explosion.")]
    [MinValue(0.0f)]
    public float UpwardsModifier;

    [Tooltip("The force mode to use upon explosion.")]
    public ForceMode ForceMode;

    [Header("Timing & Detection")]
    [Tooltip("Time in seconds from activation until the explosion occurs.")]
    [Range(0.0f, 10.0f)]
    public float DetonationTime;

    [Tooltip("Which layers will be affected by the explosion damage.")]
    public LayerMask TargetLayers;

}