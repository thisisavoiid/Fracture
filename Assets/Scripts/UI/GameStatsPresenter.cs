using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class GameStatsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreLabel;

    [Button]
    public void Present()
    {
        GameStats stats = GetGameStats();

        _scoreLabel.text = stats.Score.ToString();
    }

    private GameStats GetGameStats()
    {
        if (GameStatsManager.Instance == null)
            return new GameStats();
        
        return GameStatsManager.Instance.GetStats();
    }
}
