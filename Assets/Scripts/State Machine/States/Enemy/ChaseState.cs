using UnityEngine;

public class ChaseState : State
{
    private EnemyBrain _brain;
    public override void Enter(GameObject gameObject)
    {
        _brain = gameObject.GetComponent<EnemyBrain>();
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
    }

    public override void Exit(GameObject gameObject)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(GameObject gameObject)
    {
        _brain.Agent.SetDestination(_brain.TargetTransform.position);
        _brain.Transform.LookAt(_brain.TargetTransform.position);

        float distance = (_brain.TargetTransform.position - _brain.Transform.position).magnitude;

        if (distance >= _brain.CalmDownDistance)
        {
            _brain.SetState(new PatrolState());
            return;
        }

        if (distance < _brain.MinAttackDistance)
        {
            _brain.SetState(new AttackState());
            return;
        }
    }
}