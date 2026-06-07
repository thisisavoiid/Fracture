using NaughtyAttributes;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public void SetCursorVisible(bool value) => Cursor.visible = value;
    public void SetCursorLocked(bool value)
    {
        CursorLockMode mode = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.lockState = mode;
    }
    
    [Button]
    public void ToggleCursorLocked()
    {
        CursorLockMode currMode = Cursor.lockState;
        CursorLockMode targetMode = currMode == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;

        Cursor.lockState = targetMode;
    }
}