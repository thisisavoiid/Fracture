using UnityEngine;

/// <summary>
/// Handles the battery charging logic while the robot is at a charging station.
/// </summary>
public class ChargeBatteryState : State
{
    private Battery _battery;

    public ChargeBatteryState(Battery battery)
    {
        _battery = battery;
    }

    public override void Enter() { }

    public override void Exit() { }

    /// <summary>
    /// Executes the charging process every tick.
    /// </summary>
    public override void Run()
    {
        _battery.Charge();
    }
}