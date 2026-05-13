using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    public void PlaySoundPooled(Sound sound)
    {
        if (sound == null)
            return;

        AudioManager audioManagerInstance = AudioManager.Instance;

        if (audioManagerInstance == null)
            return;

        audioManagerInstance.PlaySound(sound, transform.position);
    }

    public void PlaySoundLocal(Sound sound)
    {
        if (sound == null)
            return;

        if (_audioSource == null)
            return;

        sound.Config.ApplyTo(_audioSource);
        _audioSource.Play();
    }
}