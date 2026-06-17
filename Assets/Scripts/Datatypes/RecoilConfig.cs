using System;
using UnityEngine;

[Serializable]
public struct RecoilConfig
{
    [Header("Intensity Parameters")]
    [Tooltip("The intensity of the vertical recoil.")]
    public float IntensityY;

    [Space(20)]
    [Tooltip("The range in which horizontal recoil is being applied randomly.")]
    public Range RandomXRecoilRange;

    [Header("Force Settings")]
    [Tooltip("The force used to pull the recoil back to its default roation.")]
    public float ForceBackToDefault;

    [Tooltip("The force used to pull the recoil to its target rotation.")]
    public float ForceToTargetForce;

    [Header("Scope Settings")]
    [Tooltip("The amount of recoil reduced once scoping is active.")]
    public float ScopeRecoilMultiplicator;
}