using System;
using UnityEngine;


[Serializable]
public struct ExplosionContext
{
    public Explosive Explosive;
    public OverlapSphereDetector OverlapSphereDetector;
    public Transform Transform;
    public GameObject GameObject;
}