using System;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    public abstract void Use(Vector3 origin, Vector3 direction, bool held, bool pressed);
    public abstract void Reload();
}