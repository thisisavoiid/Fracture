using UnityEngine;

public abstract class GunBehaviour : ScriptableObject
{
    public abstract bool Shoot(GunContext gunCtx, out RaycastHit hit);
}