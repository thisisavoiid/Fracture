using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class Swarm : MonoBehaviour
{
    public bool IsAttacking {get;}
    public abstract void Init(SwarmContext ctx);
    public abstract void StartAttack();
    public abstract void InvokeDeath();
}