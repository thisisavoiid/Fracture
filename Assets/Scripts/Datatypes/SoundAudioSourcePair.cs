using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public struct SoundAudioSourcePair
{
    [SerializeField]
    private Sound _sound;
    public Sound Sound => _sound;

    [SerializeField] 
    private AudioSource _source;
    public AudioSource Source => _source;
}