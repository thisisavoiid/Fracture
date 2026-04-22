using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/States/Enemy/Attack State", fileName = "Attack State")]
public class AttackState : State
{
    private Transform _targetTransform;
    private RayCastDetector _rayCastDetector;
    public override void Enter(StateMachineController controller)
    {
        Debug.Log($"[STATE] {GetType().Name} Enter invoked -");
        _rayCastDetector = controller.gameObject.GetComponent<RayCastDetector>();
        _targetTransform = controller.TargetTransform;
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