using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Gun Config", menuName = "Gun Config/New Gun Config", order = 1)]
public class GunConfig : ItemConfiguration
{
    [Header("Technical Specifications")]
    [Tooltip("The core balance values like damage, range, and fire rate.")]
    public WeaponStats Stats;

    [Header("Recoil Configuration")]
    [Tooltip("Configuration for recoil behaviour when this gun is being shot.")]
    public RecoilConfig RecoilConfig;
    [Space(20)]
    public bool UseRecoil = true;

    [Header("Audio")]
    [Tooltip("Configuration for the sound played when firing.")]
    public Sound ShootSound;

    [Tooltip("Configuration for the sound played when reloading.")]
    public Sound ReloadSound;

    [Header("Logic & Visuals")]
    [Tooltip("The specialized behavior script that defines how this gun operates.")]
    public GunBehaviour Behaviour;

    [Tooltip("The prefab or controller responsible for the physical projectile.")]
    public ProjectileController Projectile;
}