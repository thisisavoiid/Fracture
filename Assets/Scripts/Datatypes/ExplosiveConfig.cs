using System;
using UnityEngine;

[Serializable]
public struct ExplosiveConfig
{
    public float Radius;
    public float Damage;
    public float DetonationTime;
    public LayerMask TargetLayers;
}