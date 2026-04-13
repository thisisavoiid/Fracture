using System;
using UnityEngine;
[Serializable]
public struct GunContext
{
    public RayCastDetector RayCastDetector;
    public GunBulletTracker BulletTracker;
    public Timer Timer;
    public Gun Gun;
    public Vector3 Direction;
    public Vector3 Origin;
    public bool IsPressed;
    public bool IsHeld;
}