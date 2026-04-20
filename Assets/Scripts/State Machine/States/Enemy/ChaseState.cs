using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/States/Enemy/Chase State", fileName = "Chase State")]
public class ChaseState : State
{
    public override void Enter(StateMachineController controller)
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");

        Debug.Log("Enemy is now going to chase the player...");
    }

    public override void Exit(StateMachineController controller)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(StateMachineController controller)
    {
        controller.Agent.SetDestination(controller.TargetTransform.position);
        controller.transform.LookAt(controller.TargetTransform.position);
    }
}