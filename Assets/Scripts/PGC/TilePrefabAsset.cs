using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural Generation/Tile Prefab Asset/New Tile Prefab Asset")]
public class TilePrefabAsset : ScriptableObject
{
    [SerializeField]
    [ReorderableList]
    [BoxGroup("Tile Prefab List")]
    private List<BetterKeyValuePair<ArenaItemType, GameObject>> _tilePrefabs = new();

    public Dictionary<ArenaItemType, GameObject> GetTilePrefabMap()
    {
        Dictionary<ArenaItemType, GameObject> tilePrefabMap = new();

        foreach (BetterKeyValuePair<ArenaItemType, GameObject> typePrefabPair in _tilePrefabs)
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
                
            _tilePrefabs.Add(
                new BetterKeyValuePair<ArenaItemType, GameObject>(
                    key: (ArenaItemType)arenaItemType,
                    value: null
                )
            );
        }
    }
}