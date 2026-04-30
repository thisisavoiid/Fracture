using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Gun", menuName = "Gun/New Gun", order = 1)]
public class Gun : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("The display name of the weapon used in the UI.")]
    public string Name;

    [Header("Technical Specifications")]
    [Tooltip("The core balance values like damage, range, and fire rate.")]
    public GunConfig Stats;

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