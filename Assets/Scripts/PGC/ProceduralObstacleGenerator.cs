using System.Collections.Generic;
using System.Diagnostics;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum CellType
{
    Wall,
    Laser,
    Spawner
}

public class Cell
{
    public Cell()
    {
        foreach (int cellType in typeof(CellType).GetEnumValues())
        {
            Types.Add((CellType)cellType);
        }
    }

    public List<CellType> Types = new();

    private Dictionary<CellType, float> _cellWeightMap = new()
    {
        {CellType.Laser, 0.25f},
        {CellType.Wall, 0.3f},
        {CellType.Spawner, 0.1f}
    };

    public void Collapse()
    {
        float totalWeight = 0.0f;

        foreach (CellType cellType in this.Types)
            totalWeight += _cellWeightMap[cellType];

        float randomIndex = Random.Range(0.0f, totalWeight);

        CellType targetCellType = this.Types[0];

        foreach (CellType cellType in this.Types)
        {
            totalWeight -= _cellWeightMap[cellType];

            if (randomIndex >= totalWeight)
            {
                targetCellType = cellType;
                break;
            }
        }

        Types.Clear();
        Types.Add(targetCellType);
    }

    public void Update(CellType otherType)
    {
        if (this.Types.Count <= 1)
            return;

        switch (otherType)
        {
            case CellType.Wall:
                this.Types.Remove(CellType.Wall);
                break;

            case CellType.Laser:
                break;

            case CellType.Spawner:
                this.Types.Remove(CellType.Laser);
                break;
        }
    }
}

public class ProceduralObstacleGenerator : MonoBehaviour
{
    [BoxGroup("Grid Settings")]
    [SerializeField]
    private Vector2Int _size = Vector2Int.zero;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    private Vector2 _noiseOffset = Vector2.zero;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    [Range(0.1f, 2.0f)]
    private float _obstacleWeight;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    [Range(0.1f, 2.0f)]
    private float _noiseScale = 0.25f;

    [BoxGroup("Colors")]
    [SerializeField]
    private Color _wallColor;

    [BoxGroup("Colors")]
    [SerializeField]
    private Color _noObjectColor;

    [BoxGroup("Colors")]
    [SerializeField]
    private Color _laserColor;

    [BoxGroup("Visualization")]
    [Header("Visualization")]
    [SerializeField]
    private Sprite _tileSprite;

    [BoxGroup("Visualization")]
    [SerializeField]
    private Transform _visualizerParent;

    [BoxGroup("Seed")]
    [SerializeField]
    [ReadOnly]
    private int _currentSeed;

    [BoxGroup("Seed")]
    [SerializeField]
    private bool _useCustomSeed = false;

    [BoxGroup("Seed")]
    [SerializeField]
    [MinValue(int.MinValue)]
    [MaxValue(int.MaxValue)]
    [ShowIf("_useCustomSeed")]
    private int _customSeed;

    [BoxGroup("Seed")]
    [SerializeField]
    private bool _enableCopySeed = false;

    private Cell[,] _mapCells;
    private int[,] _obstaclePositions;

    [Button]
    [ShowIf("_enableCopySeed")]
    private void CopyCurrentSeed()
    {
        GUIUtility.systemCopyBuffer = _currentSeed.ToString();
        _enableCopySeed = false;
        Debug.Log($"<color=green>[PROCEDURAL OBSTACLE GENERATOR] Copied the current seed {_currentSeed} to your clipboard! -</color>");
    }

    [Button]
    public void Generate()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        if (_useCustomSeed)
        {
            _currentSeed = _customSeed;
        }
        else
        {
            _currentSeed = SeedGenerator.GenerateSeed();
        }

        Random.InitState(_currentSeed);

        InitializeObstacleMap(_size.x, _size.y);
        GenerateObstaclePositions();

        List<KeyValuePair<Cell, Vector2Int>> obstacleCells = FindAllObstacleCells();

        while (true)
        {
            bool hasNewCellBeenFound = CheckForCellsWithLowestEntropy(out KeyValuePair<Cell, Vector2Int> cellInfo);

            if (!hasNewCellBeenFound)
                break;

            Cell cell = cellInfo.Key;
            Vector2Int pos = cellInfo.Value;

            cell.Collapse();
            UpdateNeighbours(cell, pos);
        }

        stopwatch.Stop();

        Debug.Log($"[PROCEDURAL OBSTACLE GENERATOR] Generation done. Process took {stopwatch.Elapsed.TotalMilliseconds}ms -");

        stopwatch.Reset();
        stopwatch.Start();

        VisualizeData();

        Debug.Log($"[PROCEDURAL OBSTACLE GENERATOR] Visualization done. Process took {stopwatch.Elapsed.TotalMilliseconds}ms -");

        stopwatch.Stop();
    }

    private void InitializeObstacleMap(int sizeX, int sizeY)
    {
        _mapCells = new Cell[sizeY, sizeX];
        _obstaclePositions = new int[sizeY, sizeX];

        for (int y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++)
            {
                _mapCells[y, x] = null;
                _obstaclePositions[y, x] = 0;
            }
        }
    }

    private float GenerateNoise(Vector2Int pos)
    {
        float result = Mathf.PerlinNoise(
            (pos.x + _noiseOffset.x) * _noiseScale,
            (pos.y + _noiseOffset.y) * _noiseScale
        );

        return result;
    }

    private void GenerateObstaclePositions()
    {
        for (int y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++)
            {
                Vector2Int cellPos = new Vector2Int(x, y);
                float noiseValue = GenerateNoise(cellPos);

                if (noiseValue <= _obstacleWeight)
                {
                    _mapCells[y, x] = new Cell();
                    _obstaclePositions[y, x] = 1;
                }
            }
        }
    }

    private List<KeyValuePair<Cell, Vector2Int>> FindAllObstacleCells()
    {
        List<KeyValuePair<Cell, Vector2Int>> obstacleCells = new();

        for (int y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++)
            {
                if (_mapCells[y, x] == null)
                    continue;

                obstacleCells.Add(
                    new KeyValuePair<Cell, Vector2Int>(
                        _mapCells[y, x],
                        new Vector2Int(x, y)
                    )
                );
            }
        }

        return obstacleCells;
    }

    private void UpdateNeighbours(Cell cell, Vector2Int pos)
    {
        if (pos.y + 1 < _mapCells.GetLength(0))
            _mapCells[pos.y + 1, pos.x]?.Update(cell.Types[0]);

        if (pos.y > 0)
            _mapCells[pos.y - 1, pos.x]?.Update(cell.Types[0]);

        if (pos.x + 1 < _mapCells.GetLength(1))
            _mapCells[pos.y, pos.x + 1]?.Update(cell.Types[0]);

        if (pos.x > 0)
            _mapCells[pos.y, pos.x - 1]?.Update(cell.Types[0]);
    }

    private bool CheckForCellsWithLowestEntropy(out KeyValuePair<Cell, Vector2Int> cellInfo)
    {
        Cell lowestEntropyCell = null;
        Vector2Int lowestEntropyCellPosition = new Vector2Int(-1, -1);
        for (int y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++)
            {
                if (_mapCells[y, x] == null)
                    continue;

                if (_mapCells[y, x].Types.Count <= 1)
                    continue;

                if (lowestEntropyCell == null || _mapCells[y, x].Types.Count < lowestEntropyCell.Types.Count)
                {
                    lowestEntropyCellPosition = new Vector2Int(x, y);
                    lowestEntropyCell = _mapCells[y, x];
                }
            }
        }

        cellInfo = new KeyValuePair<Cell, Vector2Int>(
            lowestEntropyCell,
            lowestEntropyCellPosition
        );

        if (lowestEntropyCell == null)
            return false;

        return true;
    }

    private void VisualizeData()
    {
        if (_mapCells == null)
            return;

        if (_visualizerParent.childCount > 0)
        {
            for (int i = _visualizerParent.childCount - 1; i >= 0; i--)
            {
                GameObject child = _visualizerParent.GetChild(i).gameObject;

                if (child == null)
                    continue;

#if UNITY_EDITOR
                DestroyImmediate(child);
#else
                Destroy(child);
#endif
            }
        }

        for (int y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++)
            {
                GameObject tile = new GameObject($"Tile (x: {x} | y: {y}");

                tile.transform.position = new Vector3(
                    transform.position.x + x,
                    transform.position.y + y,
                    transform.position.z
                );

                tile.transform.parent = _visualizerParent;

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = _tileSprite;
                sr.color = Color.white;

                Cell cell = _mapCells[y, x];

                if (cell == null)
                {
                    sr.color = _noObjectColor;
                    continue;
                }

                CellType cellType = cell.Types[0];

                switch (cellType)
                {
                    case CellType.Wall:
                        sr.color = _wallColor;
                        break;

                    case CellType.Laser:
                        sr.color = _laserColor;
                        break;

                    case CellType.Spawner:
                        sr.color = Color.green;
                        break;
                }
            }
        }

    }

}