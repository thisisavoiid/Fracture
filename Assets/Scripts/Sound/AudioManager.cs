using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A singleton-based manager that handles global audio playback using an object pooling system. 
/// It optimizes performance by reusing <see cref="AudioSource"/> components and provides 
/// multiple overloads for playing sounds at specific locations or attached to targets.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Pool Configuration")]
    [Tooltip("The amount of audio channel pool sources to be generated.")]
    [SerializeField] [Range(1, 200)] private int _audioChannelCount;

    private List<AudioSource> _audioSourcePool = new();
    private static AudioManager _instance;

    /// <summary>
    /// Provides global access to the <see cref="AudioManager"/> singleton instance.
    /// </summary>
    /// <returns>The static instance of the <see cref="AudioManager"/>.</returns>
    public static AudioManager Instance => _instance;

    /// <summary>
    /// Initializes the audio pool and implements the singleton pattern to ensure only 
    /// one instance exists across scenes.
    /// </summary>
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

    /// <summary>
    /// Instantiates and configures the initial set of <see cref="AudioSource"/> objects 
    /// based on the <see cref="_audioChannelCount"/>.
    /// </summary>
    private void InitializeAudioPool()
    {
        for (int i = 0; i < _audioChannelCount; i++)
        {
            GameObject newPooledAudioSource = new GameObject($"PooledAudioSource_{i+1}");
            newPooledAudioSource.transform.parent = this.transform;

            AudioSource audioSource = newPooledAudioSource.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            _audioSourcePool.Add(audioSource);

            Debug.Log($"[AUDIO MANAGER] Created audio pool source {i + 1} -");
        }
    }

    /// <summary>
    /// Applies a <see cref="SoundConfig"/> to a specific <see cref="AudioSource"/>, 
    /// handling pitch/volume randomization and spatial audio settings.
    /// </summary>
    /// <param name="config">The configuration data for the sound.</param>
    /// <param name="source">The target source to configure.</param>
    private void SetupAudioSourceConfig(SoundConfig config, AudioSource source)
    {
        if (config.Clip != source.clip)
            source.clip = config.Clip;

        source.loop = config.Loop;

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
        {
            source.spatialBlend = config.SpatialBlend;
            source.rolloffMode = config.RolloffMode;
            source.minDistance = config.MinDistance;
            source.maxDistance = config.MaxDistance;
            source.dopplerLevel = config.Doppler;
            source.spread = config.Spread;
            source.reverbZoneMix = config.ReverbZoneMix;
        }
        else
            source.spatialBlend = 0.0f;
    }

    /// <summary>
    /// Searches the pool for the first <see cref="AudioSource"/> that is not currently playing.
    /// </summary>
    /// <returns>An available <see cref="AudioSource"/> or null if all are occupied.</returns>
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

    /// <summary>
    /// Plays a sound globally (non-spatial) using an available pooled source.
    /// </summary>
    /// <param name="sound">The <see cref="Sound"/> object containing data to play.</param>
    public void PlaySound(Sound sound)
    {
        Debug.Log($"[AUDIO MANAGER] Checking and setting up sound configuration -");

        AudioSource targetSource = FindUnoccupiedAudioSource();

        if (targetSource == null)
        {
            Debug.LogError($"[AUDIO MANAGER] No unoccupied AudioSource objects found. Increase AudioSource pool size to resolve this issue -");
            return;
        }

        targetSource.transform.parent = transform;

        SetupAudioSourceConfig(sound.Data, targetSource);

        Debug.Log($"[AUDIO MANAGER] Playing sound '{sound.Data.Clip.name}' -");

        targetSource.Play();
    }

    /// <summary>
    /// Plays a sound at a specific world position.
    /// </summary>
    /// <param name="sound">The <see cref="Sound"/> object containing data to play.</param>
    /// <param name="position">The <see cref="Vector3"/> world coordinates for the sound.</param>
    public void PlaySound(Sound sound, Vector3 position)
    {   
        Debug.Log($"[AUDIO MANAGER] Checking and setting up sound configuration -");

        AudioSource targetSource = FindUnoccupiedAudioSource();

        if (targetSource == null)
        {
            Debug.LogError($"[AUDIO MANAGER] No unoccupied AudioSource objects found. Increase AudioSource pool size to resolve this issue -");
            return;
        }

        targetSource.transform.parent = transform;
        targetSource.gameObject.transform.position = position;

        SetupAudioSourceConfig(sound.Data, targetSource);

        Debug.Log($"[AUDIO MANAGER] Playing sound '{sound.Data.Clip.name}' -");

        targetSource.Play();
    }

    /// <summary>
    /// Plays a sound attached to a specific <see cref="Transform"/>, allowing it to move with the target.
    /// </summary>
    /// <param name="sound">The <see cref="Sound"/> object containing data to play.</param>
    /// <param name="targetTransform">The parent transform to attach the audio source to.</param>
    public void PlaySound(Sound sound, Transform targetTransform)
    {
        Debug.Log($"[AUDIO MANAGER] Checking and setting up sound configuration -");

        AudioSource targetSource = FindUnoccupiedAudioSource();

        if (targetSource == null)
        {
            Debug.LogError($"[AUDIO MANAGER] No unoccupied AudioSource objects found. Increase AudioSource pool size to resolve this issue -");
            return;
        }

        SetupAudioSourceConfig(sound.Data, targetSource);

        targetSource.transform.parent = targetTransform;
        targetSource.transform.localPosition = Vector3.zero;

        Debug.Log($"[AUDIO MANAGER] Playing sound '{sound.Data.Clip.name}' -");

        targetSource.Play();
    }
}