using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    [SerializeField] protected ItemConfiguration _config;
    public ItemConfiguration Config => _config;
    public abstract void Use(Vector3 origin, Vector3 dir, bool held, bool pressed);
}