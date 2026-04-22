using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    private EnemyBrain _brain;
    public override void Enter(GameObject gameObject)
    {
        _brain = gameObject.GetComponent<EnemyBrain>();
        _brain.Agent.ResetPath();

        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {
        bool canSeePlayer = _brain.CanSeePlayer();

        if (canSeePlayer)
        {
            _brain.SetState(new ChaseState());
            return;
        }
    }
}