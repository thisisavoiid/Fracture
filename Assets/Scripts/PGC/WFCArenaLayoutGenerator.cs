using Unity.VisualScripting;
using UnityEngine;

public class WFCArenaLayoutGenerator : IArenaLayoutGenerator
{
    private readonly ArenaGenerationRulesAsset _rules;
    private readonly float _noiseScale;
    private readonly float _threshold;
    private readonly Vector2 _noiseOffset;

    public WFCArenaLayoutGenerator(
        ArenaGenerationRulesAsset rules,
        float noiseScale,
        float threshold,
        Vector2 noiseOffset
    )
    {
        this._rules = rules;
        this._noiseScale = noiseScale;
        this._threshold = threshold;
        this._noiseOffset = noiseOffset;
    }

    public ArenaCellData[,] Generate(Vector2Int size, int seed)
    {
        Random.InitState(seed);

        Vector2 seedOffset = new Vector2(
            Random.Range(-10000, 10000),
            Random.Range(-10000, 10000)
        );

        ArenaCellData[,] arenaCellData = new ArenaCellData[size.x, size.y];

        for (int y = 0; y < size.x; y++)
        {
            for (int x = 0; x < size.y; x++)
            {
                Vector2Int cellPos = new Vector2Int(x, y);

                arenaCellData[y, x] = new ArenaCellData();
                arenaCellData[y, x].Position = cellPos;

                float noiseValue = Mathf.PerlinNoise(
                    (cellPos.x + _noiseOffset.x + seedOffset.x) * _noiseScale,
                    (cellPos.y + _noiseOffset.y + seedOffset.y) * _noiseScale
                );

                if (noiseValue <= this._threshold)
                {
                    ArenaCellData cellData = arenaCellData[y, x];

                    foreach (int arenaItemType in typeof(ArenaItemType).GetEnumValues()) 
                        cellData.Types.Add((ArenaItemType)arenaItemType);
                }
        
                if (arenaCellData[y, x] == null || arenaCellData[y, x].Types.Count == 0)
                    arenaCellData[y, x].Types.Add(ArenaItemType.None);
            }
        }

        while (true)
        {
            ArenaCellData cellDataWithLowestEntropy = null;

            for (int y = 0; y < size.x; y++)
            {
                for (int x = 0; x < size.y; x++)
                {
                    ArenaCellData cellData = arenaCellData[y, x];

                    if (cellData == null)
                        continue;

                    if (cellData.Types.Count <= 1)
                        continue;

                    if (cellDataWithLowestEntropy == null || cellData.Types.Count < cellDataWithLowestEntropy.Types.Count)
                        cellDataWithLowestEntropy = cellData;
                }
            }

            if (cellDataWithLowestEntropy == null)
                break;

            float totalWeight = 0.0f;

            foreach (ArenaItemType arenaItemType in cellDataWithLowestEntropy.Types)
            {
                foreach (TypeRule typeRule in this._rules.Rules)
                {
                    if (typeRule.Type == arenaItemType)
                    {
                        totalWeight += typeRule.Weight;
                    }
                }
            }

            float randomIndex = Random.Range(0.0f, totalWeight);

            ArenaItemType targetArenaItemType = cellDataWithLowestEntropy.Types[0];

            foreach (ArenaItemType arenaItemType in cellDataWithLowestEntropy.Types)
            {
                foreach (TypeRule typeRule in this._rules.Rules)
                {
                    if (typeRule.Type == arenaItemType)
                    {
                        totalWeight -= typeRule.Weight;

                        if (randomIndex >= totalWeight)
                        {
                            targetArenaItemType = arenaItemType;
                            break;
                        }
                    }
                }

            }

            cellDataWithLowestEntropy.Types.Clear();
            cellDataWithLowestEntropy.Types.Add(targetArenaItemType);

            Vector2Int cellPos = cellDataWithLowestEntropy.Position;
            ArenaItemType cellType = cellDataWithLowestEntropy.Types[0];

            if (cellPos.y + 1 < arenaCellData.GetLength(0))
                UpdateCell(arenaCellData[cellPos.y + 1, cellPos.x], cellType);

            if (cellPos.y > 0)
                UpdateCell(arenaCellData[cellPos.y - 1, cellPos.x], cellType);

            if (cellPos.x + 1 < arenaCellData.GetLength(1))
                UpdateCell(arenaCellData[cellPos.y, cellPos.x + 1], cellType);

            if (cellPos.x > 0)
                UpdateCell(arenaCellData[cellPos.y, cellPos.x - 1], cellType);
        }

        return arenaCellData;
    }

    private void UpdateCell(ArenaCellData cellData, ArenaItemType originType)
    {
        if (cellData.Types.Count <= 1)
            return;

        foreach (TypeRule typeRule in this._rules.Rules)
        {
            if (typeRule.Type != originType)
                continue;

            foreach (ArenaItemType arenaItemType in typeRule.ExcludesFromNeighbors)
            {
                if (cellData.Types.Contains(arenaItemType))
                {
                    cellData.Types.Remove(arenaItemType);
                }
            }
        }
    }
}