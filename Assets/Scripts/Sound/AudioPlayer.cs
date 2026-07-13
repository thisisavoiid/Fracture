using DG.Tweening;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    private AudioSource _currentOccupiedAudioSource; 

    public void PlaySoundPooled(Sound sound)
    {
        if (sound == null)
            return;

        AudioManager audioManagerInstance = AudioManager.Instance;

        if (audioManagerInstance == null)
            return;

        AudioSource sourceUsed = audioManagerInstance.PlaySound(sound, transform.position);

        _currentOccupiedAudioSource = sourceUsed;
    }

    public void PlaySoundLocal(Sound sound)
    {
        if (sound == null)
            return;

        if (_audioSource == null)
            return;

        sound.Config.ApplyTo(_audioSource);

        if (_audioSource.enabled == false)
            return;

        if (_audioSource.gameObject.activeInHierarchy == false)
            return;

        _audioSource.Play();

        _currentOccupiedAudioSource = _audioSource;
    }

    public void StopSound(bool useFadeOut)
    {
        if (_currentOccupiedAudioSource == null)
            return;
        
        if (useFadeOut) 
            _currentOccupiedAudioSource.DOFade(0.0f, 1.0f);

        _currentOccupiedAudioSource.Stop();
    }
}