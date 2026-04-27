using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class Swarm : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack
    }

    public abstract void SetData(
        List<Swarm> swarmInstances,
        LayerMask swarmLayers,
        TransformVariable targetTransform,
        UnityAction<Swarm> onSwarmDeathEvent,
        Swarm leaderSwarm
    );
    public abstract void SetPosition(Vector3 startPos);
    public abstract void StartChase();
    public abstract void InvokeDeath();
    public abstract void SwarmTick();
}