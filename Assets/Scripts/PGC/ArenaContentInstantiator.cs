using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ArenaContentInstantiator : MonoBehaviour, IArenaContentInstantiator
{
    [BoxGroup("Prefab & Hierarchy Setup")]
    [SerializeField]
    [Expandable]
    private TilePrefabAsset _tilePrefabAsset;

    [BoxGroup("Prefab & Hierarchy Setup")]
    [SerializeField]
    private Transform _generatedObjectsParent;

    public void Build(ArenaGenerationData data)
    {
        if (_tilePrefabAsset == null)
        {
            Debug.LogError($"[ARENA CONTENT INSTANTIATOR] No tile prefab asset has been assigned! -");
            return;
        }

        if (_generatedObjectsParent == null)
        {
            Debug.LogError($"[ARENA CONTENT INSTANTIATOR] No content parent assigned! -");
            return;
        }

        Dictionary<ArenaItemType, GameObject> typePrefabMap = _tilePrefabAsset.GetTilePrefabMap();

        if (typePrefabMap == null)
        {
            Debug.LogError($"[ARENA CONTENT INSTANTIATOR] Received an invalid prefab map. Check for invalid instances in the tile prefab asset script! -");
            return;
        }

        for (int x = 0; x < data.Layout.GetLength(0); x++)
        {
            for (int z = 0; z < data.Layout.GetLength(1); z++)
            {
                ArenaCellData cellData = data.Layout[z, x];

                if (!typePrefabMap.ContainsKey(cellData.Types[0]) || typePrefabMap[cellData.Types[0]] == null)
                {
                    if (cellData.Types[0] != 0)
                        Debug.LogWarning($"[ARENA CONTENT INSTANTIATOR] Prefab at position: {cellData.Position} is null! Make sure to assign it within the tile prefab asset! -");
                    
                    continue;
                }

                Vector3 tilePositionInWorldSpace = GridPositionToWorldPosition(cellData.Position, data);

                GameObject targetPrefab = typePrefabMap[cellData.Types[0]];

                Debug.Log($"[ARENA CONTENT INSTANTIATOR] Placing prefab {typePrefabMap[cellData.Types[0]]} at position: [world] {tilePositionInWorldSpace} | [grid] {cellData.Position} -");

                GameObject tileObj = Instantiate(targetPrefab);
                tileObj.transform.parent = _generatedObjectsParent.transform;
                tileObj.transform.position = tilePositionInWorldSpace;
            }
        }
    }

    private Vector3 GridPositionToWorldPosition(Vector2 gridPosition, ArenaGenerationData data)
    {
        float halfWidth = (data.Layout.GetLength(1) - 1) * data.CellSize / 2f;
        float halfHeight = (data.Layout.GetLength(0) - 1) * data.CellSize / 2f;

        Vector3 offset = new Vector3(
            -halfWidth,
            0f,
            -halfHeight
        );

        offset += data.GlobalOffset;

        Vector3 worldPosition = new Vector3(
            transform.position.x + gridPosition.x * data.CellSize + offset.x,
            transform.position.y + offset.y,
            transform.position.z + gridPosition.y * data.CellSize + offset.z
        );

        return worldPosition;
    }

    public void Clear()
    {
        if (_generatedObjectsParent == null)
        {
            Debug.LogError($"[ARENA CONTENT INSTANTIATOR] No content parent assigned! -");
            return;
        }

        int childCount = _generatedObjectsParent.transform.childCount;

        if (childCount == 0)
        {
            Debug.Log($"[ARENA CONTENT INSTANTIATOR] The content parent object has no children, therefore, nothing has been removed. -");
            return;
        }

        GameObject[] childObjCollection = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childObjCollection[i] = _generatedObjectsParent.GetChild(i).gameObject;
        }

        int objDestroyedCount = 0;

        foreach (GameObject childObj in childObjCollection)
        {
            if (childObj == null)
                continue;

            DestroyImmediate(childObj);
            objDestroyedCount++;
        }

        Debug.Log($"[ARENA CONTENT INSTANTIATOR] {objDestroyedCount} / {childCount} child objects have been destroyed. ({childCount - objDestroyedCount} remaining) -");
    }
}