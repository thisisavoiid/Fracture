using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class Swarm : MonoBehaviour
{
    public abstract void Init(Vector3 startPos);
    public abstract void StartAttack();
    public abstract void InvokeDeath();
    public abstract void SwarmTick();
}