using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class DronePatrolState : State
{
    private DroneBrain _brain;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private NavMeshPointGenerator _navMeshPointGenerator;
    private float _speed;

    public DronePatrolState(
        DroneBrain brain,
        Rigidbody rb,
        NavMeshAgent agent,
        float speed,
        NavMeshPointGenerator navMeshPointGenerator
    )
    {
        _brain = brain;
        _rb = rb;
        _agent = agent;
        _speed = speed;
        _navMeshPointGenerator = navMeshPointGenerator;
    }

    public override void Enter()
    {
        _brain.ResetRotation();
        _agent.ResetPath();
    }

    public override void Exit()
    {
        _brain.ResetRotation();
        _agent.ResetPath();
    }

    public override void Run(float deltaTime)
    {
        if (_agent.remainingDistance <= 1.25f && !_agent.pathPending)
        {
            Vector3 randomPatrolPoint = _navMeshPointGenerator.FindPosition(50);
            _agent.SetDestination(randomPatrolPoint);
        }

        Vector3 force = _brain.CalculateForce();
        _rb.AddForce(force * _speed);
        _rb.rotation = Quaternion.LookRotation(_agent.transform.forward, Vector2.up);
    }
}