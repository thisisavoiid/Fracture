using System.Collections;
using UnityEngine;
using UnityEngine.Pool;


public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] [Range(1,250)] private int _defaultPoolSize;
    [SerializeField] [Range(1,250)] private int _maxPoolSize;
    [SerializeField] private bool _enableHardCap = false;
    private ObjectPool<AudioSource> _objectPool;

    private void Awake()
    {
        _objectPool = new(
        createFunc: CreateObject,
        actionOnGet: OnGet,
        actionOnRelease: OnRelease,
        actionOnDestroy: OnDestroyItem,
        collectionCheck: true,
        defaultCapacity: _defaultPoolSize,
        maxSize: _maxPoolSize
    );
    }

    private AudioSource CreateObject()
    {
        GameObject newGameObject = new GameObject();
        AudioSource audioSource = newGameObject.AddComponent<AudioSource>();
        audioSource.gameObject.SetActive(false);

        newGameObject.transform.parent = this.gameObject.transform;

        return audioSource;
    }

    private void OnGet(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(true);
    }

    private void OnRelease(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(false);
    }

    private void OnDestroyItem(AudioSource audioSource)
    {
        Destroy(audioSource.gameObject);
    }

    private IEnumerator ReturnAfter(AudioSource audioSource, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _objectPool.Release(audioSource);
    }

    public void SetReleaseTime(AudioSource audioSource, float seconds)
    {
        StartCoroutine(ReturnAfter(audioSource, seconds));
    }

    public AudioSource Get()
    {
        if (_objectPool.CountActive >= _maxPoolSize && _enableHardCap)
        {
            return null;
        }

        AudioSource source = _objectPool.Get();
        return source;
    }
}