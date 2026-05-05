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
    private Battery _battery;
    private bool _hasAlreadyAttackedBefore = false;

    public AttackState(
        ItemSlotController slotController,
        Transform headTransform,
        Transform targetTransform,
        Timer reloadTimer,
        Battery battery
    )
    {
        _itemSlotController = slotController;
        _headTransform = headTransform;
        _targetTransform = targetTransform;
        _reloadTimer = reloadTimer;
        _battery = battery;
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
    }

    public override void Exit() { }

    /// <summary>
    /// Manages the look-at logic, item usage, and reload cycles.
    /// </summary>
    public override void Run()
    {
        Usable equippedItem = _itemSlotController.GetEquippedItem();
        if (equippedItem == null || _headTransform == null) return;

        // Aim at the target
        _itemSlotController.transform.LookAt(_targetTransform.position);

        // Handle firing frequency via timer
        if (_reloadTimer != null && (_reloadTimer.GetRemainingTime().TotalSeconds <= 0 || !_hasAlreadyAttackedBefore))
        {
            equippedItem.Use(_headTransform.position, _headTransform.forward.normalized, true, false);
            
            if (_battery != null)
                _battery.Drain();
        }

        // Handle reload logic for Weapons
        if (equippedItem is Weapon weapon && _bulletTracker != null && !_bulletTracker.HasBulletsLeft())
        {
            weapon.Reload();
            _reloadTimer?.Reset();
            _hasAlreadyAttackedBefore = true;
        }
    }
}