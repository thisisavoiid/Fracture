using NaughtyAttributes;
using UnityEngine;

public class StatsSaveLoadHelper : SaveLoadHelper
{
    SaveLoadData oldData;

    public override void LoadCallback(SaveLoadData data)
    {
        oldData = data;
    }

    public override void SaveCallback(SaveLoadData data)
    {
        if (oldData == null)
            return;
        
        GameStats matchStats = GameStatsManager.Instance.GetStats();

        int matchScore = matchStats.Score;
        int matchExplosivesUsed = matchStats.ExplosivesUsed;
        int matchEnemiesKilled = matchStats.EnemiesKilled;

        data.TotalScore = matchScore + oldData.TotalScore;
        data.TotalExplosivesUsed = matchExplosivesUsed + oldData.TotalExplosivesUsed;
        data.TotalEnemiesKilled = matchEnemiesKilled + oldData.TotalEnemiesKilled;
    }
}
