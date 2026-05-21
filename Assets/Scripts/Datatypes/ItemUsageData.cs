using System;
using UnityEngine;

[Serializable]
public struct ItemUsageData
{
    public ItemUsageData(Vector3 origin, Vector3 forward, bool isHeld, bool isPressed)
    {
        Origin = origin;
        Direction = forward;
        IsHeld = isHeld;
        IsPressed = isPressed;
    }

    public Vector3 Origin;
    public Vector3 Direction;
    public bool IsHeld;
    public bool IsPressed;
}