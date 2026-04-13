using System;

[Serializable]
public struct GunConfig
{
    public int ShotsPerMinute;
    public float DamagePerShot;
    public int TotalRounds;
    public float Range;

    public override string ToString()
    {
        return $"GunConfig:\n" +
               $"- ShotsPerMinute: {ShotsPerMinute}\n" +
               $"- DamagePerShot: {DamagePerShot}\n" +
               $"- TotalRounds: {TotalRounds}\n" +
               $"- Range: {Range}m";
    }
}