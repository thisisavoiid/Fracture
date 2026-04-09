using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct SoundConfig
{
    public AudioClip Clip;
    public AudioMixerGroup MixerChannel;
    public float DefaultVolume;
    public float DefaultPitch;
    public bool UseRandomVolume;
    public float RandomVolumeRange;
    public bool UseRandomPitch;
    public float RandomPitchRange;
}