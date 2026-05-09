using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
[Serializable]
public struct GunContext
{
    public GameObject Holder;
    public RayCastDetector RayCastDetector;
    public GunBulletTracker BulletTracker;
    public Timer Timer;
    public GunConfig Gun;
    public Vector3 Direction;
    public Vector3 Origin;
    public Transform ProjectileSpawnTransform;
    public bool IsPressed;
    public bool IsHeld;
}