using TMPro;
using UnityEngine;

public class MainMenuStatsSaveLoadHelper : SaveLoadHelper
{
    [SerializeField] private IntDisplay _scoreDisplay;
    [SerializeField] private IntDisplay _enemiesKilledDisplay;
    [SerializeField] private IntDisplay _explosivesUsedDisplay;

    public override void LoadCallback(SaveLoadData data)
    {
        _scoreDisplay?.RefreshLabel(data.TotalScore);
        _enemiesKilledDisplay?.RefreshLabel(data.TotalEnemiesKilled);
        _explosivesUsedDisplay?.RefreshLabel(data.TotalExplosivesUsed);
    }

    public override void SaveCallback(SaveLoadData data) {}
}