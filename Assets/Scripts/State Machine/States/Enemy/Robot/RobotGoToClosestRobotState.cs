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
        Debug.Log($"[STATE MACHINE] Entering state: {this.GetType().Name.ToUpper()}.");
    }

    public override void Exit()
    {
        Debug.Log($"[STATE MACHINE] Exiting state: {this.GetType().Name.ToUpper()}.");
        _agent.ResetPath();
    }

    public override void Run(float deltaTime)
    {
        Collider[] surroundingRobotColliders = _brain.GetSurroundingRobotColliders();
        _closestTarget = GetClosestTransform(surroundingRobotColliders);

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

    private Transform GetClosestTransform(Collider[] colliders)
    {
        if (colliders.Count() == 0 || colliders == null)
            return null;
        
        Collider[] collidersSortedByDistance = colliders
        .OrderBy(
            obj => Vector3.Distance(obj.transform.root.position, _agent.transform.root.position)
        ).ToArray();

        if (collidersSortedByDistance == null || collidersSortedByDistance.Count() == 0)
            return null;
        
        Transform closestTransform = collidersSortedByDistance[0].transform;
        return closestTransform;
    }
}