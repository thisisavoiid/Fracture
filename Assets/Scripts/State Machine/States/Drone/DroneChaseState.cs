using UnityEngine;
using UnityEngine.AI;

public class DroneChaseState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Transform _target;
    private float _speed;

    public DroneChaseState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        Transform target,
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

    }

    public override void Exit()
    {

    }

    public override void Run()
    {
        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force * _speed);
        _agent.SetDestination(_target.position);
    }
}