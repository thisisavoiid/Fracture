using UnityEngine;
using UnityEngine.AI;

public class DroneChaseState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private TransformVariable _target;
    private float _speed;

    public DroneChaseState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        TransformVariable target,
        float speed
    )
    {
        _brain = brain;
        _rb = rb;
        _agent = agent;
        _target = target;
        _speed = speed;
    }

    public override void Enter()
    {
        _brain.ResetRotation();
    }

    public override void Exit()
    {

    }

    public override void Run(float deltaTime)
    {
        if (_target == null)
            return;

        if (_target.Value == null) 
            return;

        _brain.RotateTowardsTarget();

        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force * _speed);
        _agent.SetDestination(_target.Value.position);
    }
}