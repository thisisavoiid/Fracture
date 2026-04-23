using UnityEngine;

public class EventTester : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _event;

    private void Start()
    {
        _event.Invoke();
    }
}
