using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
    private ItemSlotController _itemSlotController;
    private Transform _headTransform;
    private Transform _targetTransform;
    private GunBulletTracker _bulletTracker;
    private Timer _reloadTimer;
    private Battery _battery;
    private bool _hasAlreadyAttackedBefore = false;
    private NavMeshAgent _agent;

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

    public override void Enter()
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");

        if (_itemSlotController == null)
            return;

        Usable activeItem = _itemSlotController.GetEquippedItem();

        if (activeItem is Weapon weapon)
            _bulletTracker = weapon.GetComponent<GunBulletTracker>();
    }

    public override void Exit()
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run()
    {
        Usable equippedItem = _itemSlotController.GetEquippedItem();

        if (equippedItem == null)
            return;

        if (_headTransform == null)
            return;

        
        _itemSlotController.transform.LookAt(_targetTransform.position);

        if (_reloadTimer != null && (_reloadTimer.GetRemainingTime().TotalSeconds <= 0 || !_hasAlreadyAttackedBefore))
        {
            equippedItem.Use(
                _headTransform.position,
                _headTransform.forward.normalized,
                true,
                false
            );

            if (_battery != null)
                _battery.Drain();

            if (!_hasAlreadyAttackedBefore)
                _hasAlreadyAttackedBefore = true;
        }

        if (equippedItem is not Weapon weapon)
            return;

        if (_bulletTracker == null)
            return;

        if (_reloadTimer == null)
            return;

        if (!_bulletTracker.HasBulletsLeft())
        {
            weapon.Reload();
            _reloadTimer.Reset();
        }
    }
}