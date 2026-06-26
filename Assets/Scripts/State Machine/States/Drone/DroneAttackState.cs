using UnityEngine;
using UnityEngine.AI;

public class DroneAttackState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private TransformVariable _target;
    private GunController _gunController;
    private Transform _bulletOrigin;

    public DroneAttackState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        TransformVariable target,
        Transform bulletOrigin,
        GunController gunController
    )
    {
        _brain = brain;
        _rb = rb;
        _agent = agent;
        _target = target;
        _gunController = gunController;
        _bulletOrigin = bulletOrigin;
    }

    public override void Enter()
    {
        Debug.Log("enter attack state");
        _agent.ResetPath();
    }

    public override void Exit()
    {

    }

    public override void Run()
    {
        if (_target == null)
            return;
            
        if (_target.Value == null) 
            return;
        
        _brain.RotateTowardsTarget();

        ItemUsageData itemData = new ItemUsageData(
            _bulletOrigin.position,
            _bulletOrigin.forward,
            true,
            false
        );

        _gunController.Use(itemData);
    }
}