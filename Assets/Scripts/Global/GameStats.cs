using System;
using System.Collections.Generic;
using NaughtyAttributes;

[Serializable]
public class GameStats
{
    public int Score => _score;
    private int _score = 0;

    public int ExplosivesUsed => _explosivesUsed;
    private int _explosivesUsed = 0;

    public int EnemiesKilled => _enemiesKilled;
    private int _enemiesKilled = 0;

    public void AddScore(int value) => _score += value;
    public void LogExplosiveUsed() => _explosivesUsed++;
    public void LogKilledEnemy() => _enemiesKilled++;

    public void Reset()
    {
        _score = 0;
        _enemiesKilled = 0;
        _explosivesUsed = 0;
    }
}