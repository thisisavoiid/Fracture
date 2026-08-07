using Unity.VisualScripting;
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

    public bool IsSceneValid(int index, out Scene scene)
    {
        Scene targetScene = SceneManager.GetSceneByBuildIndex(index);
        scene = targetScene;
        return targetScene != null;
    }

    public void LoadSceneByIndex(int index)
    {
        if (!IsSceneValid(index, out _))
            return;

        SceneManager.LoadScene(index);
    }

    public void UnloadSceneByIndex(int index)
    {
        if (!IsSceneLoaded(index))
            return;
        
        SceneManager.UnloadSceneAsync(index);
    }

    public void LoadSceneAdditiveByIndex(int index)
    {
        if (!IsSceneValid(index, out _))
            return;
        
        if (IsSceneLoaded(index))
            return;

        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }

    public bool IsSceneLoaded(int index)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(index);

        if (!IsSceneValid(index, out _))
            return false;
        
        return scene.isLoaded;
    }

    private void InvokeOnSceneLoadFinishedEvent(Scene scene, LoadSceneMode test) => _onSceneLoadFinished?.Invoke();
}
