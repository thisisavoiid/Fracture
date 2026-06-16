using TMPro;
using UnityEngine;

public class ItemStatsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _damageLabel;
    [SerializeField] private TextMeshProUGUI _rangeLabel;
    [SerializeField] private TextMeshProUGUI _magSizeLabel;
    [SerializeField] private TextMeshProUGUI _shotsPerMinuteLabel;

    public void PresentStats(Item item)
    {
        _nameLabel.text = item.Config.Name;
        
        if (!(item is Weapon))
            return;
        
        GunConfig gunConfig = item.Config as GunConfig;

        _damageLabel.text = gunConfig.Stats.DamagePerShot.ToString();
        _rangeLabel.text = gunConfig.Stats.Range.ToString();
        _magSizeLabel.text = gunConfig.Stats.TotalRounds.ToString();
        _shotsPerMinuteLabel.text = gunConfig.Stats.ShotsPerMinute.ToString();
    }

}
