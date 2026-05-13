using UnityEngine;
using UnityEngine.Events;

public class SoundSourceTest : MonoBehaviour
{
    [SerializeField] private UnityEvent OnSpacePressed;
    private void Update()
    {
        bool isSpacePressed = Input.GetKeyDown(KeyCode.Space);
        if (isSpacePressed)
            OnSpacePressed?.Invoke();
    }
}
