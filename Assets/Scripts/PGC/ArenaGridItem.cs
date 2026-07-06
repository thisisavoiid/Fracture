using System;
using UnityEngine;

[Serializable]
public class ArenaGridItem
{
    public ArenaGridItem(GameObject instance, ArenaItemType type, Vector2Int gridPosition)
    {
        this.Instance = instance;
        this.Type = type;
        this.GridPosition = gridPosition;
    }

    public GameObject Instance;
    public ArenaItemType Type;
    public Vector2Int GridPosition;
}