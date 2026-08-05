using System;
using UnityEngine;

[Serializable]
public struct GenerationConstraint
{
    public ArenaItemType Type;

    [Min(1)]
    public int MinQuota;
}