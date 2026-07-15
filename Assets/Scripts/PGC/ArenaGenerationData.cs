using System;
using UnityEngine;

[Serializable]
public class ArenaGenerationData
{
    public ArenaGenerationData(ArenaCellData[,] layout, float cellSize, Vector3 globalOffset)
    {
        this.Layout = layout;
        this.CellSize = cellSize;
        this.GlobalOffset = globalOffset;
    }

    public ArenaCellData[,] Layout;
    public float CellSize;
    public Vector3 GlobalOffset;
}