using UnityEngine;

public class EventInvoker : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _event;

    private void Start()
    {
        if (_event == null)
            return;

        _event.Invoke();
    }
}
