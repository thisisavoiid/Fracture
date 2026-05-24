using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSourcePool))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    private AudioSourcePool _pool;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        _pool = GetComponent<AudioSourcePool>();
    }

    public void PlaySound(Sound sound, Vector3 position)
    {
        if (sound == null)
            return;

        if (sound.Config.Clips.Count == 0 || sound.Config.Clips == null)
            return;

        AudioSource source = _pool.Get();

        sound.Config.ApplyTo(source);
        _pool.SetReleaseTime(source, source.clip.length);

        if (source.transform.parent != this.gameObject.transform)
            source.transform.parent = this.gameObject.transform;

        source.gameObject.transform.position = position;
        
        source.Play();
    }

}