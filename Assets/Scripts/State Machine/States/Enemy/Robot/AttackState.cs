using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
    private InventoryManager _inventory;
    private Transform _headTransform;
    private Transform _targetTransform;
    private GunBulletTracker _bulletTracker;
    private Timer _reloadTimer;
    private TimeMS _reloadDuration;
    private Battery _battery;
    private bool _hasAlreadyAttackedBefore = false;
    private float _rotateToTargetSnappiness;

    public AttackState(
        InventoryManager inventory,
        Transform headTransform,
        Transform targetTransform,
        TimeMS reloadDuration,
        Timer reloadTimer,
        Battery battery,
        float rotateToTargetSnappiness
    )
    {
        _inventory = inventory;
        _headTransform = headTransform;
        _targetTransform = targetTransform;
        _reloadTimer = reloadTimer;
        _battery = battery;
        _reloadDuration = reloadDuration;
        _rotateToTargetSnappiness = rotateToTargetSnappiness;

        _reloadTimer.SetTime(_reloadDuration);
        _reloadTimer.StartTimer();
    }

    public override void Enter()
    {
        if (_inventory == null) return;

        Item activeItem = _inventory.ActiveItem;

        if (activeItem is Weapon weapon)
            _bulletTracker = weapon.GetComponent<GunBulletTracker>();

        if (_reloadTimer == null)
            return;
    }

    public override void Exit() { }

    public override void Run()
    {
        if (_inventory == null)
            return;

        if (_headTransform == null)
            return;

        if (_targetTransform == null)
            return;

        Item equippedItem = _inventory.ActiveItem;

        Vector3 lookDir = (_targetTransform.position - _headTransform.position).normalized;
        lookDir.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir);

        _inventory.transform.rotation = Quaternion.Lerp(
            _inventory.transform.rotation,
            targetRotation,
            Time.deltaTime * _rotateToTargetSnappiness
        );

        if (equippedItem == null)
            return;
            
        if (_reloadTimer != null)
        {
            if (_reloadTimer.GetRemainingTime().TotalSeconds <= 0 || !_hasAlreadyAttackedBefore)
            {
                ItemUsageData usageData = new ItemUsageData(
                    _headTransform.position,
                    _headTransform.forward.normalized,
                    true,
                    false
                );

                _inventory.UseActiveItem(usageData);

                if (_battery != null)
                    _battery.Drain();
            }
        }


        if (equippedItem is Weapon weapon)
        {
            if (_bulletTracker == null)
                return;

            if (!_bulletTracker.HasBulletsLeft())
            {
                weapon.Reload();
                _reloadTimer?.Reset();
                _hasAlreadyAttackedBefore = true;
            }
        }
    }
}