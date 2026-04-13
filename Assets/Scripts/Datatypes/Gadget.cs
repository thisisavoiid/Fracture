using UnityEngine;

public abstract class Gadget : Usable
{
    public abstract void PrimaryUse(Vector3 origin, Vector3 dir);
    public abstract void SecondaryUse(Vector3 origin, Vector3 dir);
}