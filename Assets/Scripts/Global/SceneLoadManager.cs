using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private UnityEvent _onSceneLoadFinished;
    
    private void Awake()
    {
        SceneManager.sceneLoaded += InvokeOnSceneLoadFinishedEvent;
    }

    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void InvokeOnSceneLoadFinishedEvent(Scene scene, LoadSceneMode test) => _onSceneLoadFinished?.Invoke();
}
