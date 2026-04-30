using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct SoundConfig
{
    [Header("Core Settings")]
    [Tooltip("The audio clip to be played.")]
    public AudioClip Clip;

    [Tooltip("The output mixer group for this sound.")]
    public AudioMixerGroup MixerChannel;

    [Header("Standard Playback")]
    [Tooltip("The baseline volume of the audio clip.")]
    [Range(0.0f, 2.0f)] public float DefaultVolume;

    [Tooltip("The baseline pitch of the audio clip.")]
    [Range(0.0f, 2.0f)] public float DefaultPitch;

    [Header("Randomization")]
    [Tooltip("Whether to apply a random offset to the volume every time the sound plays.")]
    public bool UseRandomVolume;

    [Tooltip("The maximum range for volume randomization.")]
    [Range(0.0f, 1.0f)] public float RandomVolumeRange;

    [Space(5)]
    [Tooltip("Whether to apply a random offset to the pitch every time the sound plays.")]
    public bool UseRandomPitch;

    [Tooltip("The maximum range for pitch randomization.")]
    [Range(0.0f, 1.0f)] public float RandomPitchRange;

    [Header("3D Audio Settings")]
    [Tooltip("Enables 3D spatial sound processing.")]
    public bool UseSpatialAudio;

    [Tooltip("Sets how much the 3D engine affects the sound (0 = 2D, 1 = 3D).")]
    [Range(0.0f, 1.0f)] public float SpatialBlend;
}