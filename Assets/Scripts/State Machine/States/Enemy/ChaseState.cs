using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/States/Enemy/Chase State", fileName = "Chase State")]
public class ChaseState : State
{
    public override void Enter()
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");

        Debug.Log("Enemy is now going to chase the player...");
    }

    public override void Exit()
    {
        Debug.Log($"[STATE] {GetType().Name} Exit invoked -");
    }

    public override void Update()
    {
        // Debug.Log($"[STATE] {GetType().Name} Update invoked -");
    }
}