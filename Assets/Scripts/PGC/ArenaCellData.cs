using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ArenaCellData
{
    public ArenaCellData()
    {
        this.Position = Vector2Int.zero;
        this.Types = new List<ArenaItemType>();
    }

    public ArenaCellData(Vector2Int position, List<ArenaItemType> types)
    {
        this.Position = position;
        this.Types = types;
    }
    public Vector2Int Position;
    public List<ArenaItemType> Types;
}