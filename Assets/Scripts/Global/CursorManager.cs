using NaughtyAttributes;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private bool _setDefaultModeOnAwake = false;

    [SerializeField]
    [Foldout("Default Cursor Lock Mode")]
    [ShowIf("_setDefaultModeOnAwake")]
    private CursorLockMode _cursorModeOnAwake = CursorLockMode.Locked;

    private void Awake()
    {
        if (!_setDefaultModeOnAwake)
            return;

        Cursor.lockState = _cursorModeOnAwake;
    }

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