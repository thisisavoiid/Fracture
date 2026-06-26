using UnityEngine;
using UnityEngine.AI;

public class DroneIdleState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private float _speed;

    public DroneIdleState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        float speed
    )
    {
        _brain = brain;
        _rb = rb;
        _agent = agent;
        _speed = speed;
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
        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force * _speed);
    }
}