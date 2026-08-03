using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationConstraintsApplicator
{
    public void ApplyConstraints(ArenaCellData[,] layout, List<GenerationConstraint> constraints)
    {
        if (layout == null)
            return;
        
        if (constraints == null)
            return;
        
        List<Vector2Int> unoccupiedPositions = FetchUnoccupiedCellPositions(layout);
        Dictionary<ArenaItemType, int> remainingConstraintsCount = new();

        foreach (GenerationConstraint constraint in constraints)
        {
            int existingCount = CountExistingCellType(layout, constraint.Type);
            int missingCount = constraint.MinQuota - existingCount;

            if (missingCount > 0)
                remainingConstraintsCount[constraint.Type] = missingCount;
        }

        if (remainingConstraintsCount.Count == 0 || GetTotalRemainingConstraintQuotas(remainingConstraintsCount) <= 0)
        {
            Debug.Log("[GENERATION CONSTRAINTS APPLICATOR] All constraints have already been satisfied: No further filling needed! -");
            return;
        }

        for (int i = unoccupiedPositions.Count; i > 0; i--)
        {
            if (GetTotalRemainingConstraintQuotas(remainingConstraintsCount) <= 0)
                return;

            int posIndex = Random.Range(0, unoccupiedPositions.Count);
            int lastIndex = unoccupiedPositions.Count - 1;

            Vector2Int pos = unoccupiedPositions[posIndex];

            unoccupiedPositions[posIndex] = unoccupiedPositions[lastIndex];
            unoccupiedPositions.RemoveAt(lastIndex);

            ArenaItemType priorityCellType = GetCellTypeWithHighestPriority(remainingConstraintsCount);

            layout[pos.y, pos.x].Types.Clear();
            layout[pos.y, pos.x].Types.Add(priorityCellType);

            remainingConstraintsCount[priorityCellType]--;
        }
    }

    private int CountExistingCellType(ArenaCellData[,] layout, ArenaItemType type)
    {
        int count = 0;

        for (int x = 0; x < layout.GetLength(1); x++)
        {
            for (int y = 0; y < layout.GetLength(0); y++)
            {
                ArenaCellData cell = layout[y, x];

                if (cell.Types.Count > 0 && cell.Types[0] == type)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private int GetTotalRemainingConstraintQuotas(Dictionary<ArenaItemType, int> remainingConstraints)
    {
        int sum = 0;

        foreach (KeyValuePair<ArenaItemType, int> pair in remainingConstraints)
        {
            sum += pair.Value;
        }

        return sum;
    }

    private ArenaItemType GetCellTypeWithHighestPriority(Dictionary<ArenaItemType, int> remainingConstraints)
    {
        return remainingConstraints.OrderByDescending(obj => obj.Value).First(obj => obj.Value > 0).Key;
    }

    private List<Vector2Int> FetchUnoccupiedCellPositions(ArenaCellData[,] layout)
    {
        List<Vector2Int> positions = new();

        for (int x = 0; x < layout.GetLength(1); x++)
        {
            for (int y = 0; y < layout.GetLength(0); y++)
            {
                ArenaCellData cell = layout[y, x];

                if (cell.Types.Count == 0)
                    continue;

                if (cell.Types[0] != ArenaItemType.None)
                    continue;

                positions.Add(cell.Position);
            }
        }

        return positions;
    }
}