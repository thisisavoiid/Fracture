
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private int _audioChannelCount;
    private List<AudioSource> _audioSourcePool = new();
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    private void Awake()
    {
        InitializeAudioPool();

        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitializeAudioPool()
    {
        for (int i = 0; i < _audioChannelCount; i++)
        {
            GameObject newPooledAudioSource = new GameObject($"PooledAudioSource_{i}");
            newPooledAudioSource.transform.parent = this.transform;

            AudioSource audioSource = newPooledAudioSource.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            _audioSourcePool.Add(audioSource);

            Debug.Log($"[AUDIO MANAGER] Created audio pool source {i + 1} -");
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

        if (config.UseSpatialAudio)
            source.spatialBlend = 1.0f;
        else
            source.spatialBlend = 0.0f;

    }

    private AudioSource FindUnoccupiedAudioSource()
    {
        AudioSource targetSource = null;

        foreach (AudioSource source in _audioSourcePool)
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

    public void PlaySound(Sound sound, Vector3 position)
    {
        Debug.Log($"[AUDIO MANAGER] Checking and setting up sound configuration -");

        AudioSource targetSource = FindUnoccupiedAudioSource();

        if (targetSource == null)
        {
            Debug.LogError($"[AUDIO MANAGER] No unoccupied AudioSource objects found. Increase AudioSource pool size to resolve this issue -");
            return;
        }

        targetSource.gameObject.transform.position = position;

        SetupAudioSourceConfig(sound.Data, targetSource);

        Debug.Log($"[AUDIO MANAGER] Playing sound '{sound.Data.Clip.name}' -");

        targetSource.Play();
    }

}
