using UnityEngine;
using UnityEngine.Events;

public class GunBulletTracker : MonoBehaviour
{
    public UnityEvent<int> OnBulletCountChange;
    private GunConfig _gun;
    private int _bulletsRemaining;
    public int BulletsRemaining => _bulletsRemaining;
    
    public void ResetBullets(GunConfig gun)
    {
        if (_gun == null)
            LoadGunData(gun);

        _bulletsRemaining = _gun.Stats.TotalRounds;

        OnBulletCountChange?.Invoke(_bulletsRemaining);

        Debug.Log($"[GUN BULLET TRACKER] Resetting bullets of {_gun.Name} to default: {_gun.Stats.TotalRounds} -");
    }

    public bool HasBulletsLeft()
    {
        return _bulletsRemaining > 0;
    }
    
    private void LoadGunData(GunConfig gun)
    {
        _gun = gun;
        _bulletsRemaining = _gun.Stats.TotalRounds;
    }

    public void DecreaseRemainingBulletCount(GunConfig gun)
    {
        if (_gun == null)
            LoadGunData(gun);

        if (_bulletsRemaining <= 0)
        {
            return;
        }
            
        _bulletsRemaining--;

        OnBulletCountChange.Invoke(_bulletsRemaining);
        
        Debug.Log($"[GUN BULLET TRACKER] {_gun.Name} fired a shot: {_bulletsRemaining} / {_gun.Stats.TotalRounds} bullets remaining -");
    }
}