using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : State
{
    private EnemyBrain _brain;
    private GunBulletTracker _bulletTracker;
    public override void Enter(GameObject gameObject)
    {
        _brain = gameObject.GetComponent<EnemyBrain>();

        Usable activeGun = _brain.ItemSlotController.GetEquippedItem();
        _bulletTracker = activeGun.GetComponent<GunBulletTracker>();
    
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {
        Usable equippedItem = _brain.ItemSlotController.GetEquippedItem();

        if (equippedItem == null)
            return;

        if (_brain.HeadTransform == null)
            return;

        _brain.Transform.LookAt(_brain.TargetTransform.position);

        equippedItem.Use(
            _brain.HeadTransform.position,
            _brain.HeadTransform.forward.normalized,
            true,
            false
        );
        
        float distance = (_brain.TargetTransform.position - _brain.Transform.position).magnitude;

        if (distance > _brain.MinAttackDistance)
        {
            _brain.SetState(new ChaseState());
            return;
        }
        
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