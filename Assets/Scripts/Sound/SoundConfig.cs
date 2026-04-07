using System;
using UnityEngine;

[Serializable]
public struct SoundConfig
{
    public AudioClip Clip;
    public bool Loop;
    public float DefaultVolume;
    public float DefaultPitch;
    public bool UseRandomVolume;
    public float MaxRandomVolume;
    public float MinRandomVolume;
    public bool UseRandomPitch;
    public float RandomPitchRange;
}