using UnityEngine;

public interface ICanPlaySound
{
    public void PlaySound(Sound sound, bool useCustomAudioSource, AudioSource source);
}