using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SoundConfig
{
    [Header("Core Settings")]
    [Tooltip("The audio clip to be played.")]
    public AudioClip Clip;

    [Tooltip("The output mixer group for this sound.")]
    public AudioMixerGroup MixerChannel;

    [Header("Standard Playback")]
    [Tooltip("The baseline volume of the audio clip.")]
    [Range(0.0f, 2.0f)] public float DefaultVolume = 1.0f;

    [Tooltip("The baseline pitch of the audio clip.")]
    [Range(-3.0f, 3.0f)] public float DefaultPitch = 1.0f;

    [Header("Randomization")]
    [Tooltip("Whether to apply a random offset to the volume every time the sound plays.")]
    public bool UseRandomVolume = false;

    [Tooltip("The maximum range for volume randomization.")]
    [Range(0.0f, 1.0f)] public float RandomVolumeRange = 0.1f;

    [Space(5)]
    [Tooltip("Whether to apply a random offset to the pitch every time the sound plays.")]
    public bool UseRandomPitch = false;

    [Tooltip("The maximum range for pitch randomization.")]
    [Range(0.0f, 1.0f)] public float RandomPitchRange = 0.1f;

    [Header("3D Audio Settings")]
    [Tooltip("Enables 3D spatial sound processing.")]
    public bool UseSpatialAudio = false;

    [Tooltip("Sets how much the 3D engine affects the sound (0 = 2D, 1 = 3D).")]
    [Range(0.0f, 1.0f)] public float SpatialBlend = 0.0f;

    [Tooltip("Sets how the volume attenuates over distance.")]
    public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;

    [Tooltip("Within the MinDistance, the sound will stay at full volume.")]
    [Range(0.0f, 1000.0f)] public float MinDistance = 1.0f;

    [Tooltip("The distance at which the sound ceases to attenuate.")]
    [Range(0.0f, 1000.0f)] public float MaxDistance = 500.0f;

    [Tooltip("Sets the Doppler scale for this audio source.")]
    [Range(0.0f, 5.0f)] public float Doppler = 1.0f;

    [Tooltip("Sets the spread angle (in degrees) of a 3D stereo or multichannel sound in speaker space.")]
    [Range(0.0f, 360.0f)] public float Spread = 0.0f;

    [Tooltip("The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones.")]
    [Range(0.0f, 1.1f)] public float ReverbZoneMix = 1.0f;

    [Header("Loop Settings")]
    [Tooltip("Whether or not the sound should be played on loop.")]
    public bool Loop = false;
}