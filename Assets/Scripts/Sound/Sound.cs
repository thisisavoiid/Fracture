using UnityEngine;

/// <summary>
/// A ScriptableObject wrapper for <see cref="SoundConfig"/>, allowing sound definitions 
/// to be created as assets within the Unity Project.
/// </summary>
[CreateAssetMenu(fileName = "Sound", menuName = "Sounds/New Sound", order = 1)]
public class Sound : ScriptableObject
{
    [Header("Configuration")]
    [Tooltip("The underlying configuration data for this sound asset.")]
    [SerializeField] private SoundConfig _soundConfig;

    /// <summary>
    /// Provides access to the <see cref="SoundConfig"/> data.
    /// </summary>
    /// <returns>The <see cref="SoundConfig"/> associated with this sound.</returns>
    public SoundConfig Data => _soundConfig;
}