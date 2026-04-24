using UnityEngine.Events;
using UnityEngine;

public struct SwarmContext
{
    public SwarmContext(UnityAction<Swarm> onSwarmDeathAction, TransformVariable targetTransform, Vector3 startPosition)
    {
        OnSwarmDeathAction = onSwarmDeathAction;
        TargetTransform = targetTransform;
        StartPosition = startPosition;
    }

    public UnityAction<Swarm> OnSwarmDeathAction;
    public TransformVariable TargetTransform;
    public Vector3 StartPosition;
}