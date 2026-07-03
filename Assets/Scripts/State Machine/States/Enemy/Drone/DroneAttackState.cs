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
        _agent.ResetPath();
    }

    public override void Exit()
    {
        _agent.ResetPath();
    }

    public override void Run(float deltaTime)
    {
        if (_target == null)
            return;

        if (_target.Value == null)
            return;

        _brain.RotateTowardsTarget();
        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force);

        ItemUsageData itemData = new ItemUsageData(
            _bulletOrigin.position,
            _bulletOrigin.forward,
            true,
            false
        );

        _gunController.Use(itemData);
    }
}