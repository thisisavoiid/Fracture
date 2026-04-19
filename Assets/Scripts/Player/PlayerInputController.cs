using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the <see cref="GameInput"/> data and provides access to other classes.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    private GameInput _gameInput;

    private InputAction _move;
    public Vector3 Move => _move.ReadValue<Vector3>().normalized;

    private InputAction _jump;
    public bool Jump => _jump.WasPressedThisFrame();

    private InputAction _sprint;
    public bool Sprint => _sprint.IsPressed();

    public Vector2 Look => Mouse.current.delta.ReadValue();

    private InputAction _primaryGadgetAction;
    public InputAction PrimaryGadgetAction => _primaryGadgetAction;

    private InputAction _secondaryGadgetAction;
    public InputAction SecondaryGadgetAction => _secondaryGadgetAction;

    private InputAction _reloadWeapon;
    public InputAction ReloadWeapon => _reloadWeapon;

    private InputAction _shuffleInventorySlots;
    public float ShuffleInventorySlots => _shuffleInventorySlots.ReadValue<float>();

    private void Awake()
    {
        _gameInput = new GameInput();

        _move = _gameInput.Movement.Move;
        _sprint = _gameInput.Movement.Sprint;
        _jump = _gameInput.Movement.Jump;

        _primaryGadgetAction = _gameInput.Gadgets.PrimaryAction;
        _secondaryGadgetAction = _gameInput.Gadgets.SecondaryAction;

        _reloadWeapon = _gameInput.Gadgets.Reload;

        _shuffleInventorySlots = _gameInput.Inventory.Shuffle;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _gameInput.Enable();
    }

    private void OnDisable()
    {
        _gameInput.Disable();
    }
}