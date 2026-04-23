using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
    private ItemSlotController _itemSlotController;
    private Transform _headTransform;
    private Transform _targetTransform;

    public AttackState(ItemSlotController slotController, Transform headTransform, Transform targetTransform)
    {
        _itemSlotController = slotController;
        _headTransform = headTransform;
        _targetTransform = targetTransform;
    }

    public override void Enter(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {
        Usable equippedItem = _itemSlotController.GetEquippedItem();

        if (equippedItem == null)
            return;

        if (_headTransform == null)
            return;

        _itemSlotController.transform.LookAt(_targetTransform.position);

        equippedItem.Use(
            _headTransform.position,
            _headTransform.forward.normalized,
            true,
            false
        );

        // Reload-Logik => Wird noch verbessert!!

        // if (equippedItem is Weapon && _bulletTracker != null)
        // {
        //     Weapon weaponItem = equippedItem as Weapon;

        //     bool canShoot = _bulletTracker.HasBulletsLeft();

        //     if (!canShoot) 
        //         weaponItem.Reload();
        // }

    }
}