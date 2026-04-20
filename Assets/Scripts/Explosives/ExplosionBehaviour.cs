using UnityEngine;

public abstract class ExplosionBehaviour : ScriptableObject
{
    public abstract void Explode(ExplosionContext ctx);
}