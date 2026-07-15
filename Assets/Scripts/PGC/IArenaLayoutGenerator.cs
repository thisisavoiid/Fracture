using UnityEngine;

public interface IArenaLayoutGenerator
{
    ArenaCellData[,] Generate(Vector2Int size, int seed);
}