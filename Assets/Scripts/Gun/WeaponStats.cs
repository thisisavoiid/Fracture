using System;
using UnityEngine;

[Serializable]
public struct WeaponStats
{
    [Header("Firing Mechanics")]
    [Tooltip("How many rounds the gun fires in one minute.")]
    [Range(1, 2000)]
    public int ShotsPerMinute;

    [Tooltip("The amount of health or durability removed by a single projectile.")]
    [Range(0.1f, 500f)]
    public float DamagePerShot;

    [Header("Ammo & Capacity")]
    [Tooltip("The maximum amount of ammunition available.")]
    [Range(1, 1000)]
    public int TotalRounds;

    [Header("Ballistics")]
    [Tooltip("The maximum effective distance the projectile can travel in meters.")]
    [Range(1f, 2000f)]
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