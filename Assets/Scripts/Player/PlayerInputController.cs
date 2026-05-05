using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the <see cref="GameInput"/> data and provides access to other classes. This class serves 
/// as the central interface for reading player inputs such as movement, gadget actions, and 
/// inventory management.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    private GameInput _gameInput;

    private InputAction _move;
    /// <summary>
    /// Reads and returns the normalized movement vector.
    /// </summary>
    /// <returns>A <see cref="Vector3"/> representing the movement input.</returns>
    public Vector3 Move => _move.ReadValue<Vector3>().normalized;

    private InputAction _jump;
    /// <summary>
    /// Checks if the jump button was pressed this frame.
    /// </summary>
    /// <returns>A <see cref="bool"/> indicating the jump state.</returns>
    public bool Jump => _jump.WasPressedThisFrame();

    private InputAction _sprint;
    /// <summary>
    /// Checks if the sprint button is currently being held down.
    /// </summary>
    /// <returns>A <see cref="bool"/> indicating the sprint state.</returns>
    public bool Sprint => _sprint.IsPressed();

    private InputAction _crouchSlide;
    /// <summary>
    /// Checks if the crouch/slide button is currently being held down.
    /// </summary>
    /// <returns>A <see cref="bool"/> indicating the crouch or slide state.</returns>
    public bool CrouchSlide => _crouchSlide.IsPressed();

    /// <summary>
    /// Reads the current mouse delta for look mechanics.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing mouse movement.</returns>
    public Vector2 Look => Mouse.current.delta.ReadValue();

    private InputAction _primaryGadgetAction;
    /// <summary>
    /// Provides access to the primary gadget <see cref="InputAction"/>.
    /// </summary>
    /// <returns>The <see cref="InputAction"/> for primary gadget usage.</returns>
    public InputAction PrimaryGadgetAction => _primaryGadgetAction;

    private InputAction _secondaryGadgetAction;
    /// <summary>
    /// Provides access to the secondary gadget <see cref="InputAction"/>.
    /// </summary>
    /// <returns>The <see cref="InputAction"/> for secondary gadget usage.</returns>
    public InputAction SecondaryGadgetAction => _secondaryGadgetAction;

    private InputAction _reloadWeapon;
    /// <summary>
    /// Provides access to the weapon reload <see cref="InputAction"/>.
    /// </summary>
    /// <returns>The <see cref="InputAction"/> for reloading.</returns>
    public InputAction ReloadWeapon => _reloadWeapon;

    private InputAction _shuffleInventorySlots;
    /// <summary>
    /// Reads the current scroll or button value for inventory shuffling.
    /// </summary>
    /// <returns>A <see cref="float"/> representing the shuffle direction or value.</returns>
    public float ShuffleInventorySlots => _shuffleInventorySlots.ReadValue<float>();

    /// <summary>
    /// Initializes a new instance of <see cref="GameInput"/>, caches all movement, gadget, 
    /// and inventory actions, and locks the system cursor.
    /// </summary>
    private void Awake()
    {
        _gameInput = new GameInput();

        _move = _gameInput.Movement.Move;
        _sprint = _gameInput.Movement.Sprint;
        _jump = _gameInput.Movement.Jump;
        _crouchSlide = _gameInput.Movement.CrouchSlide;
        
        _primaryGadgetAction = _gameInput.Gadgets.PrimaryAction;
        _secondaryGadgetAction = _gameInput.Gadgets.SecondaryAction;

        _reloadWeapon = _gameInput.Gadgets.Reload;

        _shuffleInventorySlots = _gameInput.Inventory.Shuffle;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Enables the <see cref="GameInput"/> action map.
    /// </summary>
    private void OnEnable()
    {
        _gameInput.Enable();
    }

    /// <summary>
    /// Disables the <see cref="GameInput"/> action map.
    /// </summary>
    private void OnDisable()
    {
        _gameInput.Disable();
    }
}