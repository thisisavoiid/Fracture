using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class GameStatsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _enemiesKilledLabel;
    [SerializeField] private TextMeshProUGUI _explosivesUsedLabel;

    [Button]
    public void Present()
    {
        GameStats stats = GetGameStats();

        if (_scoreLabel != null)
            _scoreLabel.text = stats.Score.ToString();
        
        if (_enemiesKilledLabel != null)
            _enemiesKilledLabel.text = stats.EnemiesKilled.ToString();
        
        if (_explosivesUsedLabel != null)
            _explosivesUsedLabel.text = stats.ExplosivesUsed.ToString();
    }

    private GameStats GetGameStats()
    {
        if (GameStatsManager.Instance == null)
            return new GameStats();
        
        return GameStatsManager.Instance.GetStats();
    }
}
