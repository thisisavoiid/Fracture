using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Events;

public class GunBulletTracker : MonoBehaviour
{
    public UnityEvent<int> OnBulletCountChange;
    private Gun _gun;
    private int _totalBullets;
    private int _bulletsRemaining;

    public void ResetBullets()
    {
        if (_gun == null)
        {
            Debug.LogError("[GUN BULLET TRACKER] Couldn't reset bullets as Gun is still null. Use LoadGunStats() inside a GunController to counter this issue -");
            return;
        }

        _bulletsRemaining = _totalBullets;

        OnBulletCountChange.Invoke(_bulletsRemaining);

        Debug.Log($"[GUN BULLET TRACKER] Resetting bullets of {_gun.Name} to default: {_totalBullets}");
    }

    public bool HasBulletsLeft()
    {
        return _bulletsRemaining > 0;
    }
    public void DecreaseRemainingBulletCount()
    {
        if (_gun == null)
        {
            Debug.LogError("[GUN BULLET TRACKER] Couldn't decrease bullets as Gun is still null. Use LoadGunStats() inside a GunController to counter this issue -");
            return;
        }

        if (_bulletsRemaining <= 0)
        {
            return;
        }
            
        _bulletsRemaining--;

        OnBulletCountChange.Invoke(_bulletsRemaining);
        
        Debug.Log($"[GUN BULLET TRACKER] {_gun.Name} fired a shot: {_bulletsRemaining} / {_totalBullets} bullets remaining -");
    }

    public void LoadGunStats(Gun gun)
    {
        _gun = gun;
        _totalBullets = _gun.Stats.TotalRounds;
        _bulletsRemaining = _totalBullets;
    }
}