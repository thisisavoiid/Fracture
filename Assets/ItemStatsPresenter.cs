using TMPro;
using UnityEngine;

public class ItemStatsPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private TextMeshProUGUI _damageLabel;
    [SerializeField] private TextMeshProUGUI _rangeLabel;

    public void PresentStats(Item item)
    {
        _nameLabel.text = item.Config.Name;
        
        if (!(item is Weapon))
            return;
        
        GunConfig gunConfig = item.Config as GunConfig;

        _damageLabel.text = gunConfig.Stats.DamagePerShot.ToString();
        _rangeLabel.text = gunConfig.Stats.Range.ToString();
    }

}
