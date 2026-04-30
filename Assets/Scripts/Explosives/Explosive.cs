using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Explosive", menuName = "Explosives/New Explosive", order = 1)]
public class Explosive : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("The display name of the explosive device.")]
    public string Name;

    [Header("Explosion Properties")]
    [Tooltip("Core configuration for damage, radius, and timing.")]
    public ExplosiveConfig Config;

    [Header("Audio Feedback")]
    [Tooltip("Sound played when the detonation sequence or fuse starts.")]
    public Sound DetonationCycleStartSound;

    [Tooltip("Sound played at the moment of explosion.")]
    public Sound ExplodeSound;
}