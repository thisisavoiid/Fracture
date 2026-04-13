
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private int _audioChannelCount;
    private List<AudioSource> _audioSources = new();
    private void Awake()
    {
        for (int i = 0; i < _audioChannelCount; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;  
            _audioSources.Add(newSource);
            Debug.Log($"[AUDIO MANAGER] Created audio source {i + 1} -");
        }
    }

    private void SetupAudioSourceConfig(SoundConfig config, AudioSource source)
    {
        if (config.Clip != source.clip)
            source.clip = config.Clip;

        if (config.MixerChannel != source.outputAudioMixerGroup)
            source.outputAudioMixerGroup = config.MixerChannel;

        if (config.DefaultVolume != source.volume)
            source.volume = config.DefaultVolume;

        if (config.UseRandomVolume)
        {
            source.volume = Random.Range(
                    config.DefaultVolume - config.RandomVolumeRange,
                    config.DefaultVolume + config.RandomVolumeRange
            );
        }

        if (config.DefaultPitch != source.pitch)
            source.pitch = config.DefaultPitch;

        if (config.UseRandomPitch)
        {
            source.pitch = Random.Range(
                    config.DefaultPitch - config.RandomPitchRange,
                    config.DefaultPitch + config.RandomPitchRange
            );
        }
    }

    private AudioSource FindUnoccupiedAudioSource()
    {
        AudioSource targetSource = null;

        foreach (AudioSource source in _audioSources)
        {
            if (source.isPlaying)
                continue;

            targetSource = source;
            break;
        }

        return targetSource;
    }

    public void PlaySound(Sound sound)
    {
        Debug.Log($"[AUDIO MANAGER] Checking and setting up sound configuration -");

        AudioSource targetSource = FindUnoccupiedAudioSource();

        if (targetSource == null)
        {
            Debug.LogError($"[AUDIO MANAGER] No unoccupied AudioSource objects found. Increase AudioSource pool size to resolve this issue -");
            return;
        }

        SetupAudioSourceConfig(sound.Data, targetSource);

        Debug.Log($"[AUDIO MANAGER] Playing sound '{sound.Data.Clip.name}' -");

        targetSource.Play();
    }
}
