using UnityEngine;
using UnityEngine.AI;

public class DroneAttackState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Transform _target;
    private float _speed;
    private GunController _gunController;
    private Transform _bulletOrigin;

    public DroneAttackState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        Transform target,
        Transform bulletOrigin,
        GunController gunController,
        float speed

    )
    {
        _brain = brain;
        _rb = rb;
        _agent = agent;
        _target = target;
        _speed = speed;
        _gunController = gunController;
        _bulletOrigin = bulletOrigin;
    }

    public override void Enter()
    {
        _agent.ResetPath();
    }

    public override void Exit()
    {

    }

    public override void Run()
    {
        Vector3 dir = _target.position - _rb.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        _rb.rotation = Quaternion.Lerp(
            _rb.rotation,
            targetRotation,
            Time.deltaTime * 15f
        );

        ItemUsageData itemData = new ItemUsageData(
            _bulletOrigin.position,
            _bulletOrigin.forward,
            true,
            false
        );

        _gunController.Use(itemData);
    }
}