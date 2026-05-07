using UnityEngine;

public abstract class Bee : MonoBehaviour
{
    public abstract void Tick();
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetState(BeeState state);

}