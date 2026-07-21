using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Tile Prefab Asset/New Tile Prefab Asset")]
public class TilePrefabAsset : ScriptableObject
{
    [ReorderableList]
    [BoxGroup("Tile Prefab List")]
    public List<BetterKeyValuePair<ArenaItemType, GameObject>> TilePrefabs = new();

    public Dictionary<ArenaItemType, GameObject> GetTilePrefabMap()
    {
        Dictionary<ArenaItemType, GameObject> tilePrefabMap = new();

        foreach (BetterKeyValuePair<ArenaItemType, GameObject> typePrefabPair in TilePrefabs)
        {
            tilePrefabMap[typePrefabPair.Key] = typePrefabPair.Value;
        }

        return tilePrefabMap;
    }

    private void Reset()
    {
        foreach (int arenaItemType in typeof(ArenaItemType).GetEnumValues())
        {
            if (arenaItemType == 0)
                continue;
                
            TilePrefabs.Add(
                new BetterKeyValuePair<ArenaItemType, GameObject>(
                    key: (ArenaItemType)arenaItemType,
                    value: null
                )
            );
        }
    }
}