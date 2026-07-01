using UnityEngine;

public class RobotChargeBatteryState : State
{
    private Battery _battery;

    public RobotChargeBatteryState(Battery battery)
    {
        _battery = battery;
    }

    public override void Enter() { }

    public override void Exit() { }

    public override void Run(float deltaTime)
    {
        _battery.Charge(deltaTime);
    }
}