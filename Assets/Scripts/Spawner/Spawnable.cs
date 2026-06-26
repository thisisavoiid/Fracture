using System;
using UnityEngine;

[Serializable]
public abstract class Spawnable : MonoBehaviour
{
    public abstract void Spawn();
}