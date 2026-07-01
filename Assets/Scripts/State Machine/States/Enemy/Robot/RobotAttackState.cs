using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RobotAttackState : State
{
    private InventoryController _inventory;
    private NavMeshAgent _agent;
    private TransformVariable _targetTransform;
    private float _turnSpeed;
    private bool _isFiringAShot;
    public bool IsFiringAShot => _isFiringAShot;
    public RobotAttackState(
        NavMeshAgent agent,
        InventoryController inventory,
        TransformVariable targetTransform,
        float turnSpeed
    )
    {
        _inventory = inventory;
        _turnSpeed = turnSpeed;
        _targetTransform = targetTransform;
        _agent = agent;
    }

    public override void Enter()
    {
        Debug.Log($"[STATE MACHINE] Entering state: {this.GetType().Name.ToUpper()}.");
        _agent.updateRotation = false;
    }

    public override void Exit()
    {
        Debug.Log($"[STATE MACHINE] Exiting state: {this.GetType().Name.ToUpper()}.");
        _agent.updateRotation = true;
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

        bool itemUseSuccessful = _inventory.UseActiveItem(itemUsageData);
        _isFiringAShot = itemUseSuccessful;
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