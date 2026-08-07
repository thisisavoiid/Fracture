using System.Linq;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class RobotGoToClosestRobotState : State
{
    private RobotBrain _brain;
    private NavMeshAgent _agent;
    private Transform _closestTarget = null;
    public bool AnyCloseTargetFound => _closestTarget != null;
    public bool ClosestTargetReached
    {
        get
        {
            if (!AnyCloseTargetFound)
                return true;

            bool isRobotCloseToTarget = Vector3.Distance(
                _agent.transform.position,
                _closestTarget.position
            ) <= 5.0f;

            return isRobotCloseToTarget;
        }
    }
    public RobotGoToClosestRobotState(
        NavMeshAgent agent,
        RobotBrain brain
    )
    {
        _agent = agent;
        _brain = brain;
    }

    public override void Enter()
    {
        _closestTarget = null;
    }

    public override void Exit()
    {
        _closestTarget = null;
        _agent.ResetPath();
    }

    public override void Run(float deltaTime)
    {
        _closestTarget = _brain.GetClosestRobot();
        
        if (_closestTarget == null)
            return;

        if (ClosestTargetReached)
        {
            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
        }
        else
        {
            _agent.SetDestination(_closestTarget.position);
        }
    }

    
}