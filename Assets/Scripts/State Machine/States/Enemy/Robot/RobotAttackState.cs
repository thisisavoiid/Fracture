using System.Collections;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RobotAttackState : State
{
    private InventoryController _inventory;
    private NavMeshAgent _agent;
    private TransformVariable _targetTransform;
    private GunBulletTracker _bulletTracker;
    private float _turnSpeed;
    private float _cooldownTimeAfterReload;
    private float _cooldownTimePassed = 0f;
    private bool _isCooldownActive = false;
    private bool _isFiringAShot = false;
    public bool IsFiringAShot => _isFiringAShot;

    public RobotAttackState(
        NavMeshAgent agent,
        InventoryController inventory,
        TransformVariable targetTransform,
        float turnSpeed,
        TimeMS cooldownAfterReload
    )
    {
        _inventory = inventory;
        _turnSpeed = turnSpeed;
        _targetTransform = targetTransform;
        _agent = agent;
        _cooldownTimeAfterReload = cooldownAfterReload.TotalSeconds;
    }

    public override void Enter()
    {
        _agent.updateRotation = false;

        _isFiringAShot = false;

        if (_inventory.ActiveItem is Weapon weapon)
        {
            _bulletTracker = weapon.gameObject.GetComponent<GunBulletTracker>();
        }
    }

    public override void Exit()
    {
        _agent.updateRotation = true;
        _isFiringAShot = false;
    }

    public override void Run(float deltaTime)
    {
        RotateToTarget(
            _turnSpeed,
            deltaTime
        );

        ItemUsageData itemUsageData = new ItemUsageData(
            _agent.transform.position,
            _agent.transform.forward,
            true,
            false
        );

        if (_isCooldownActive) 
            _cooldownTimePassed += deltaTime;
        
        if (_cooldownTimePassed >= _cooldownTimeAfterReload)
        {
            _isCooldownActive = false;
            _cooldownTimePassed = 0.0f;
        }

        if (_isCooldownActive)
            return;
        
        bool itemUseSuccessful = _inventory.UseActiveItem(itemUsageData);
        _isFiringAShot = itemUseSuccessful; 

        if (_bulletTracker == null)
            return;
        
        if (!_bulletTracker.HasBulletsLeft())
        {
            _isFiringAShot = false;

            Weapon weapon = _inventory.ActiveItem as Weapon;
            weapon.Reload();

            _isCooldownActive = true;
        }
    }

    private void RotateToTarget(float turnSpeed, float deltaTime)
    {
        Vector3 directionToTarget = (_targetTransform.Value.position - _agent.transform.position);
        directionToTarget.y = 0;

        Quaternion currentRotation = _agent.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);

        _agent.transform.rotation = Quaternion.Lerp(
            currentRotation,
            targetRotation,
            deltaTime * turnSpeed
        );
    }
}