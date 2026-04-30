using UnityEngine;
using UnityEngine.AI;

public class ChargeBatteryState : State
{
    private Battery _battery;

    public ChargeBatteryState(Battery battery)
    {
        _battery = battery;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        
    }

    public override void Run()
    {
        _battery.Charge();
    }
}