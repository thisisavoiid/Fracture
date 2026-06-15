using NaughtyAttributes;
using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    private static GameStatsManager _instance;
    public static GameStatsManager Instance => _instance;

    private GameStats _gameStats = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void AddScore(int value)
    {
        _gameStats.AddScore(value);
    }

    public void LogKilledEnemy()
    {
        _gameStats.LogKilledEnemy();
    }

    public void LogExplosiveUsed()
    {
        _gameStats.LogExplosiveUsed();
    }
    
    public void ResetStats() => _gameStats.Reset();
    public GameStats GetStats() => _gameStats;
}
