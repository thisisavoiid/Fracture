
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Random = UnityEngine.Random;

public class ProceduralObstacleGenerationPlacer : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _gridDimensions;

    [SerializeField]
    [Range(1, 20)]
    [MinValue(1)]
    private float _cellSize;

    [SerializeField]
    private Vector3 _globalGenerationOffset = Vector3.zero;

    [SerializeField] 
    private Transform _proceduralGenerationContentParent;

    [Space(10)]

    [SerializeField]
    [Expandable]
    private List<ProceduralGenerationAsset> _generationAssets;

    [Space(10)]

    private Dictionary<Vector2Int, ArenaGridItem> _arenaGrid = new();
    private Dictionary<ArenaItemType, ProceduralGenerationAsset> _generationAssetMap = new();

    private void LoadGenerationAssets()
    {
        foreach (ProceduralGenerationAsset generationAsset in _generationAssets)
        {
            if (_generationAssetMap.ContainsKey(generationAsset.Type))
                continue;

            _generationAssetMap.Add(generationAsset.Type, generationAsset);
        }
    }

    private void SetGridItem(Vector2Int position, ArenaGridItem item)
    {
        if (_arenaGrid.ContainsKey(position))
        {
            _arenaGrid[position] = item;
        }
        else
        {
            _arenaGrid.Add(position, item);
        }
    }

    private void SnapGridItemInstancesToWorldPosition()
    {
        if (_arenaGrid == null || _arenaGrid.Count == 0)
            return;

        foreach (KeyValuePair<Vector2Int, ArenaGridItem> arenaGridItemEntry in _arenaGrid)
        {
            GameObject instance = arenaGridItemEntry.Value.Instance;

            if (instance == null)
                continue;
            
            Transform instanceTransform = instance.transform;
            Vector3 worldPosition = GridPositionToWorldPosition(arenaGridItemEntry.Value.GridPosition);

            instanceTransform.position = new Vector3(
                worldPosition.x,
                0,
                worldPosition.z
            );
        }
    }

    private Vector3 GridPositionToWorldPosition(Vector2 gridPosition)
    {
        float halfWidth = (_gridDimensions.x - 1) * _cellSize / 2f;
        float halfHeight = (_gridDimensions.y - 1) * _cellSize / 2f;

        Vector3 offset = new Vector3(
            -halfWidth,
            0f,
            -halfHeight
        );

        offset += _globalGenerationOffset;

        Vector3 worldPosition = new Vector3(
            transform.position.x + gridPosition.x * _cellSize + offset.x,
            0f,
            transform.position.z + gridPosition.y * _cellSize + offset.z
        );

        return worldPosition;
    }

    [Button]
    private void DeleteContent()
    {
        if (_arenaGrid.Keys.Count <= 0)
            return;

        foreach (Vector2Int gridPosition in _arenaGrid.Keys)
        {
            GameObject instance = _arenaGrid[gridPosition].Instance;

            if (instance == null)
                continue;

            if (!instance.gameObject.activeInHierarchy)
                continue;
#if UNITY_EDITOR
            DestroyImmediate(instance);
#else
            Destroy(instance);
#endif
        }

        _arenaGrid.Clear();
    }

    [Button]
    private void GenerateArenaContent()
    {
        if (_generationAssets == null || _generationAssets.Count == 0)
            return;

        LoadGenerationAssets();

        if (_proceduralGenerationContentParent == null)
            CreateTransformParentObject();

        DeleteContent();

        GenerateWalls();

        InstantiateAllGeneratedItemsInScene();
        SetArenaItemNames();
        SnapGridItemInstancesToWorldPosition();
        ApplyArenaItemOffsets();
    }

    private void SetArenaItemNames()
    {
        if (_arenaGrid == null || _arenaGrid.Count == 0)
            return;

        if (_generationAssetMap == null || _generationAssetMap.Count == 0)
            return;

        foreach (KeyValuePair<Vector2Int, ArenaGridItem> arenaGridItemEntry in _arenaGrid)
        {
            if (!_generationAssetMap.ContainsKey(arenaGridItemEntry.Value.Type))
                continue;

            GameObject instance = arenaGridItemEntry.Value.Instance;

            if (instance == null)
                continue;

            ArenaItemType type = arenaGridItemEntry.Value.Type;

            arenaGridItemEntry.Value.Instance.name = _generationAssetMap[type].Name;
        }
    }

    private void ApplyArenaItemOffsets()
    {
        if (_arenaGrid == null || _arenaGrid.Count == 0)
            return;

        if (_generationAssetMap == null || _generationAssetMap.Count == 0)
            return;

        foreach (KeyValuePair<Vector2Int, ArenaGridItem> arenaGridItemEntry in _arenaGrid)
        {
            if (!_generationAssetMap.TryGetValue(arenaGridItemEntry.Value.Type, out ProceduralGenerationAsset asset))
                continue;

            Transform instanceTransform = arenaGridItemEntry.Value.Instance.transform;

            if (instanceTransform == null)
                continue;

            Vector3 offset = asset.Offset + _globalGenerationOffset;
            Vector3 targetPosition = instanceTransform.position + offset;

            instanceTransform.position = targetPosition;
        }
    }

    private void CreateTransformParentObject()
    {
        GameObject parentObject = new GameObject("PGC Content");
        parentObject.transform.position = Vector3.zero;
        _proceduralGenerationContentParent = parentObject.transform;
    }

    private void GenerateWalls()
    {
        ProceduralGenerationAsset wallGenerationAsset = FindGenerationAssetByArenaItemType(ArenaItemType.Wall);

        if (wallGenerationAsset == null)
            return;

        List<GameObject> wallTilePrefabs = wallGenerationAsset.Prefabs;

        if (wallTilePrefabs == null || wallTilePrefabs.Count == 0)
            return;

        for (int x = 0; x < _gridDimensions.x; x++)
        {
            for (int z = 0; z < _gridDimensions.y; z++)
            {
                Vector2Int gridPosition = new Vector2Int(x, z);

                GameObject wall = wallTilePrefabs[Random.Range(0, wallTilePrefabs.Count)];

                if (!_arenaGrid.ContainsKey(gridPosition) || _arenaGrid[gridPosition] == null)
                {
                    ArenaGridItem wallGridItem = new ArenaGridItem(wall, ArenaItemType.Wall, gridPosition);
                    SetGridItem(gridPosition, wallGridItem);
                }
            }
        }
    }

    private ProceduralGenerationAsset FindGenerationAssetByArenaItemType(ArenaItemType type)
    {
        ProceduralGenerationAsset generationAsset = null;
        _generationAssetMap.TryGetValue(type, out generationAsset);
        return generationAsset;
    }

    private void InstantiateAllGeneratedItemsInScene()
    {
        foreach (KeyValuePair<Vector2Int, ArenaGridItem> arenaGridEntry in _arenaGrid)
        {
            GameObject notYetInstantiatedInstance = arenaGridEntry.Value.Instance;

            if (notYetInstantiatedInstance == null)
                continue;

            GameObject instantiatedInstance = Instantiate(
                notYetInstantiatedInstance,
                _proceduralGenerationContentParent
            );

            arenaGridEntry.Value.Instance = instantiatedInstance;
        }
    }

    private void OnDrawGizmos()
    {
        float halfWidth = (_gridDimensions.x - 1) * _cellSize / 2f;
        float halfHeight = (_gridDimensions.y - 1) * _cellSize / 2f;

        Vector3 offset = new Vector3(
            -halfWidth,
            0,
            -halfHeight
        );

        for (float x = 0; x < _gridDimensions.x; x++)
        {
            for (float z = 0; z < _gridDimensions.y; z++)
            {
                Vector3 pointDrawPosition = new Vector3(
                    transform.position.x + x * _cellSize + offset.x + _globalGenerationOffset.x,
                    transform.position.y,
                    transform.position.z + z * _cellSize + offset.z + _globalGenerationOffset.x
                );

                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(pointDrawPosition, 0.25f);

                Vector3 cubeDrawPosition = new Vector3(
                    transform.position.x + x * _cellSize + offset.x + _globalGenerationOffset.x,
                    transform.position.y + _globalGenerationOffset.y,
                    transform.position.z + z * _cellSize + offset.z + _globalGenerationOffset.z
                );

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(cubeDrawPosition, Vector3.one * _cellSize);

            }
        }
    }
}
