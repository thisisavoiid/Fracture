using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;
[Serializable]
public struct TypeRule
{
    [BoxGroup("Rule Configuration")]
    public ArenaItemType Type;

    [BoxGroup("Rule Configuration")]
    [Range(0.0f, 1.0f)]
    public float Weight;

    [BoxGroup("Constraints")]
    [ReorderableList]
    public List<ArenaItemType> ExcludesFromNeighbors;
}