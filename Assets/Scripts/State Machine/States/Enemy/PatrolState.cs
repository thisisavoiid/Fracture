using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/States/Enemy/Patrol State", fileName = "Patrol State")]
public class PatrolState : State
{
    public override void Enter(StateMachineController controller)
    {
        controller.Agent.destination = controller.transform.position;
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");

        Debug.Log("Enemy is now going to patrol...");
    }

    public override void Exit(StateMachineController controller)
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Run(StateMachineController controller)
    {
        // Debug.Log($"[STATE] {GetType().Name} Update invoked -");
    }
}