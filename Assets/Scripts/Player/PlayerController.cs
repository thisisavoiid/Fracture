using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central controller class responsible for managing player systems including movement, 
/// camera logic, physics interactions, and item usage. This class acts as a hub 
/// coordinating components like <see cref="RigidbodyMovement"/>, <see cref="CameraController"/>, 
/// and <see cref="ItemSlotController"/>.
/// </summary>
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(RigidbodyMovement))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(OverlapBoxDetector))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(HeadBob))]
[RequireComponent(typeof(PhysicsMaterialChanger))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private float _cameraSensitivity;
    [SerializeField] private float _cameraAngleClamp = 90.0f;

    [Header("Animators")]
    [SerializeField] private Animator _itemAnimator;
    [SerializeField] private Animator _weaponAdsAnimator;

    [Header("Field Of View")]
    [SerializeField] private float _baseFov;
    [SerializeField] private float _walkFovMultiplicator;
    [SerializeField] private float _sprintFovMultiplicator;
    [SerializeField] private float _lerpSpeed;

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("Headbobbing")]
    [SerializeField] private float _baseHeadbobStrength;
    [SerializeField] private float _baseHeadbobSpeed;
    [SerializeField] private float _walkHeadbobStrengthMultiplicator;
    [SerializeField] private float _walkHeadbobSpeedMultiplicator;
    [SerializeField] private float _sprintHeadbobStrengthMultiplicator;
    [SerializeField] private float _sprintHeadbobSpeedMultiplicator;

    [Header("Movement")]
    [Header("Speed Settings")]
    [SerializeField] private float _defaultMoveSpeed;
    [SerializeField] private float _sprintMultiplicator;
    [SerializeField] private float _slideStartSpeed;
    [SerializeField] private float _slideDeceleration;

    [Header("Slide")]
    [SerializeField][Range(0.0f, 1.0f)] private float _cameraYPosChange = 0.5f;

    [Header("Jump")]
    [SerializeField] private float _jumpStrength;

    [Header("Scriptable Variables")]
    [SerializeField] private TransformVariable _transformVariable;

    [Header("Event Endpoints")]
    [SerializeField] private UnityEvent<Vector2> _onMouseLook;
    [SerializeField] private UnityEvent<Vector3> _onMove;
    [SerializeField] private UnityEvent _onJump;
    [SerializeField] private UnityEvent _onSlideStart;
    [SerializeField] private UnityEvent _onSlideEnd;


    [Header("Phsyics Materials")]
    [SerializeField] private PhysicsMaterial _generalPhysicsMaterial;
    [SerializeField] private PhysicsMaterial _slidePhysicsMaterial;

    [Header("Various")]
    [SerializeField] private bool _isLocked = false;

    [Header("Event Endpoints")]

    private PlayerInputController _inputController;
    private RigidbodyMovement _rbMovement;
    private CameraController _cameraController;
    private OverlapBoxDetector _overlapBoxDetector;
    private InventoryManager _inventoryManager;
    private HeadBob _headbob;
    private PhysicsMaterialChanger _physicsMaterialChanger;
    private bool _isJumpQueued = false;
    private bool _isSliding = false;
    private float _currentSlideSpeed;
    private Vector3 _lastSlideDir;
    private Vector3 _moveDir;
    private bool _isGrounded;

    private void Awake()
    {
        _inputController = GetComponent<PlayerInputController>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _cameraController = GetComponent<CameraController>();
        _overlapBoxDetector = GetComponent<OverlapBoxDetector>();
        _inventoryManager = GetComponent<InventoryManager>();
        _headbob = GetComponent<HeadBob>();
        _physicsMaterialChanger = GetComponent<PhysicsMaterialChanger>();

        if (_transformVariable != null)
            _transformVariable.SetValue(transform);

        _cameraController.SetFOVLerpSpeed(_lerpSpeed);
    }

    /// <summary>
    /// Manages player movement logic by calculating movement speed based on sprint state or 
    /// handling deceleration during a slide, then applying it to the <see cref="RigidbodyMovement"/>.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 _moveDir = _inputController.Move;

        if (_isSliding)
        {
            _currentSlideSpeed -= Time.fixedDeltaTime * _slideDeceleration;
            _currentSlideSpeed = Mathf.Max(0.0f, _currentSlideSpeed);

            if (_lastSlideDir == Vector3.zero)
                return;

            _rbMovement.Move(transform.TransformDirection(_lastSlideDir), _currentSlideSpeed, true);
        }
        else
        {
            if (_moveDir == Vector3.zero)
                return;

            float targetSpeed = _defaultMoveSpeed;

            if (_inputController.Sprint)
                targetSpeed *= _sprintMultiplicator;

            _rbMovement.Move(transform.TransformDirection(_moveDir), targetSpeed, true);

            _onMove?.Invoke(_moveDir);
        }
    }

    /// <summary>
    /// Checks for jump input and queues a jump if the player is currently grounded 
    /// and not performing a slide.
    /// </summary>
    private void HandleJump()
    {
        if (_isSliding)
            return;

        bool jumpPressed = _inputController.JumpPressed;

        if (!(jumpPressed && !_isJumpQueued))
            return;

        if (_isGrounded)
            _isJumpQueued = true;
    }

    /// <summary>
    /// Manages camera rotation and player body orientation based on mouse input, 
    /// clamping the vertical pitch to the defined <see cref="_cameraAngleClamp"/>.
    /// </summary>
    private void HandleCameraLook()
    {
        Vector2 mouseDelta = _inputController.Look;
        float mouseX = mouseDelta.x * _cameraSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * _cameraSensitivity * Time.deltaTime;

        Quaternion currentRbRotation = _rbMovement.GetRotation();
        _rbMovement.SetRotation(Quaternion.Euler(0f, mouseX + currentRbRotation.eulerAngles.y, 0f));

        float pitch = Mathf.DeltaAngle(0f, _cameraController.GetLocalRotation().eulerAngles.x);

        pitch += -mouseY;
        pitch = Mathf.Clamp(pitch, -_cameraAngleClamp, _cameraAngleClamp);

        _cameraController.SetLocalRotation(Quaternion.Euler(pitch, 0f, 0f));

        _onMouseLook?.Invoke(mouseDelta);
    }

    /// <summary>
    /// Calculates and applies headbob speed and strength to the <see cref="HeadBob"/> component 
    /// based on movement state (idle, walking, or sprinting).
    /// </summary>
    private void HandleHeadbob()
    {
        if (_moveDir == Vector3.zero || _isSliding)
        {
            _headbob.SetSpeed(_baseHeadbobSpeed);
            _headbob.SetStrength(_baseHeadbobStrength);
            return;
        }

        if (_inputController.Sprint)
        {
            _headbob.SetSpeed(_baseHeadbobSpeed * _sprintHeadbobSpeedMultiplicator);
            _headbob.SetStrength(_baseHeadbobStrength * _sprintHeadbobStrengthMultiplicator);
            return;
        }

        _headbob.SetSpeed(_baseHeadbobSpeed * _walkHeadbobSpeedMultiplicator);
        _headbob.SetStrength(_baseHeadbobStrength * _walkHeadbobStrengthMultiplicator);
    }

    /// <summary>
    /// Manages the camera's target Field of View based on whether the player is 
    /// stationary, walking, or sprinting.
    /// </summary>
    private void HandleCameraFOV()
    {
        Vector3 _moveDir = _inputController.Move;

        if (_moveDir == Vector3.zero)
        {
            _cameraController.SetTargetFOV(_baseFov);
            return;
        }

        if (_inputController.Sprint)
        {
            _cameraController.SetTargetFOV(_baseFov * _sprintFovMultiplicator);
            return;
        }

        _cameraController.SetTargetFOV(_baseFov * _walkFovMultiplicator);
    }

    /// <summary>
    /// Checks for gadget action inputs and triggers the <see cref="Item.Use"/> method 
    /// on the currently equipped item from the <see cref="ItemSlotController"/>.
    /// </summary>
    private void HandleItemUse()
    {
        bool wasPrimaryGadgetActionPressed = _inputController.PrimaryGadgetAction.WasPressedThisFrame();
        bool isPrimaryGadgetActionHeldDown = _inputController.PrimaryGadgetAction.IsPressed();

        if (!(wasPrimaryGadgetActionPressed || isPrimaryGadgetActionHeldDown))
            return;

        Transform cameraTransform = _cameraController.GetTransform();

        ItemUsageData usageData = new ItemUsageData(
            cameraTransform.position,
            cameraTransform.forward,
            isPrimaryGadgetActionHeldDown,
            wasPrimaryGadgetActionPressed
        );

        _inventoryManager.UseActiveItem(usageData);
    }

    /// <summary>
    /// Checks for reload input and triggers the <see cref="Weapon.Reload"/> method 
    /// if the equipped item is of type <see cref="Weapon"/>.
    /// </summary>
    private void HandleGunReload()
    {
        Item equippedItem = _inventoryManager.ActiveItem;

        if (equippedItem == null)
            return;

        bool wasReloadPressed = _inputController.ReloadWeapon.WasPressedThisFrame();

        if (!wasReloadPressed)
            return;

        if (equippedItem is not Weapon weapon)
            return;

        weapon.Reload();
    }

    /// <summary>
    /// Manages the inventory slot selection by calculating the target index 
    /// based on input and updating the <see cref="ItemSlotController"/>.
    /// </summary>
    private void HandleInventoryShuffle()
    {
        if (_inputController.ShuffleInventorySlots == 0)
            return;

        if (_inventoryManager.Capacity == 0)
            return;

        int currentIndex = _inventoryManager.ActiveSlot;
        int targetIndex = currentIndex + (int)_inputController.ShuffleInventorySlots;
        int inventoryCapactiy = _inventoryManager.Capacity;

        if (targetIndex < 0)
        {
            targetIndex = inventoryCapactiy - 1;
        } 

        if (targetIndex >= inventoryCapactiy)
        {
            targetIndex = 0;
        }

        _inventoryManager.SetActiveSlot(targetIndex, out _);
    }

    /// <summary>
    /// Manages player sliding logic by checking whether the player isn't crouching already and changing player
    /// physics as well as the camera y position offset.
    /// </summary>
    private void HandleSlide()
    {
        bool isCrouchSlidePressed = _inputController.CrouchSlide;

        if (isCrouchSlidePressed != _isSliding)
        {
            _isSliding = isCrouchSlidePressed;

            Vector3 targetCameraLocalPos = _cameraController.GetLocalPosition();

            if (_isSliding)
            {
                _currentSlideSpeed = _slideStartSpeed;
                targetCameraLocalPos.y -= _cameraYPosChange;

                _cameraController.SetLocalPosition(targetCameraLocalPos);

                _onSlideStart?.Invoke();
            }
            else
            {
                targetCameraLocalPos.y += _cameraYPosChange;
                _cameraController.SetLocalPosition(targetCameraLocalPos);

                _onSlideEnd?.Invoke();
            }

            Vector3 _moveDir = _inputController.Move;
            _lastSlideDir = _moveDir;
        }

        _physicsMaterialChanger.SetPhysicsMaterial(isCrouchSlidePressed ? _slidePhysicsMaterial : _generalPhysicsMaterial);
    }

    /// <summary>
    /// Executes every player gameplay related method every frame. Also caches current move direction.
    /// </summary>
    private void Update()
    {
        if (_isLocked)
            return;

        HandleJump();
        HandleSlide();
        HandleCameraLook();
        HandleItemUse();
        HandleGunReload();
        HandleInventoryShuffle();
        HandleCameraFOV();
        HandleHeadbob();

        _weaponAdsAnimator.SetBool("Scope", _inputController.SecondaryGadgetAction.IsPressed());
        _moveDir = _inputController.Move;
    }

    /// <summary>
    /// Responsible for movement, ground check caching as well as animator-related variable setting.
    /// </summary>
    private void FixedUpdate()
    {

        if (_itemAnimator != null)
        {
            _itemAnimator.SetFloat("Speed", Mathf.Round(_rbMovement.CurrentVelocity.magnitude));
            _itemAnimator.SetBool("IsInAir", !_isGrounded);
        }

        if (_isLocked)
            return;

        HandleMovement();

        _isGrounded = _overlapBoxDetector.CheckForAnyObjects(_groundLayers);

        if (_isJumpQueued)
        {
            _isJumpQueued = false;
            _rbMovement.Jump(_jumpStrength);
            _onJump?.Invoke();
        }
    }

    public void SetLocked(bool value) => _isLocked = value;
}