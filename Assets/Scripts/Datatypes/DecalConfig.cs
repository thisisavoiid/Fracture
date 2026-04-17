using System;
using UnityEngine;

[Serializable] 
public struct DecalConfig
{
    public Material Material;
    public float DestroyAfter;
    public bool StayVisibleForever;
    public float DefaultSize;
    public float RandomRange;
}