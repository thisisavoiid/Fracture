using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Executes combat logic, including aiming, firing items, and reloading.
/// </summary>
public class AttackState : State
{
    private ItemSlotController _itemSlotController;
    private Transform _headTransform;
    private Transform _targetTransform;
    private GunBulletTracker _bulletTracker;
    private Timer _reloadTimer;
    private TimeMS _reloadDuration;
    private Battery _battery;
    private bool _hasAlreadyAttackedBefore = false;

    public AttackState(
        ItemSlotController slotController,
        Transform headTransform,
        Transform targetTransform,
        TimeMS reloadDuration,
        Timer reloadTimer,
        Battery battery
    )
    {
        _itemSlotController = slotController;
        _headTransform = headTransform;
        _targetTransform = targetTransform;
        _reloadTimer = reloadTimer;
        _battery = battery;
        _reloadDuration = reloadDuration;

        _reloadTimer.SetTime(_reloadDuration);
        _reloadTimer.Start();
    }

    /// <summary>
    /// Identifies the equipped weapon to track ammunition status.
    /// </summary>
    public override void Enter()
    {
        if (_itemSlotController == null) return;

        Usable activeItem = _itemSlotController.GetEquippedItem();

        if (activeItem is Weapon weapon)
            _bulletTracker = weapon.GetComponent<GunBulletTracker>();

        if (_reloadTimer == null)
            return;
    }

    public override void Exit() { }

    /// <summary>
    /// Manages the look-at logic, item usage, and reload cycles.
    /// </summary>
    public override void Run()
    {
        // Debug Checks für kritische Referenzen
        if (_itemSlotController == null)
        {
            Debug.LogError("AttackState: _itemSlotController is null!");
            return;
        }

        if (_headTransform == null)
        {
            Debug.LogError("AttackState: _headTransform is null!");
            return;
        }

        if (_targetTransform == null)
        {
            Debug.LogError("AttackState: _targetTransform is null!");
            return;
        }

        Usable equippedItem = _itemSlotController.GetEquippedItem();
        if (equippedItem == null) 
        {
            Debug.LogWarning("AttackState: No equipped item found.");
            return;
        }

        Vector3 lookDir = (_targetTransform.position - _headTransform.position).normalized;
        lookDir.y = 0f;

        _itemSlotController.transform.rotation = Quaternion.LookRotation(lookDir);

        // Handle firing frequency via timer
        if (_reloadTimer != null)
        {
            if (_reloadTimer.GetRemainingTime().TotalSeconds <= 0 || !_hasAlreadyAttackedBefore)
            {
                equippedItem.Use(_headTransform.position, _headTransform.forward.normalized, true, false);
                
                if (_battery != null)
                {
                    _battery.Drain();
                }
                else
                {
                    Debug.LogWarning("AttackState: _battery is null while trying to drain.");
                }
            }
        }
        else
        {
            Debug.LogError("AttackState: _reloadTimer is null!");
        }

        // Handle reload logic for Weapons
        if (equippedItem is Weapon weapon)
        {
            if (_bulletTracker == null)
            {
                Debug.LogWarning("AttackState: Item is weapon but _bulletTracker is null!");
            }
            else if (!_bulletTracker.HasBulletsLeft())
            {
                weapon.Reload();
                _reloadTimer?.Reset();
                _hasAlreadyAttackedBefore = true;
            }
        }
    }
}