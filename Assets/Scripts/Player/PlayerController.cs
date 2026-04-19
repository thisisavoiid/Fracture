using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(RigidbodyMovement))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(OverlapBoxDetector))]
[RequireComponent(typeof(ItemSlotController))]
[RequireComponent(typeof(HeadBob))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private float _cameraSensitivity;
    [SerializeField] private float _cameraAngleClamp = 90.0f;

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

    [Header("Jump")]
    [SerializeField] private float _jumpStrength;

    private PlayerInputController _inputController;
    private RigidbodyMovement _rbMovement;
    private CameraController _cameraController;
    private OverlapBoxDetector _overlapBoxDetector;
    private ItemSlotController _itemSlotController;
    private HeadBob _headbob;
    private bool _isJumpQueued = false;

    private void Awake()
    {
        _inputController = GetComponent<PlayerInputController>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _cameraController = GetComponent<CameraController>();
        _overlapBoxDetector = GetComponent<OverlapBoxDetector>();
        _itemSlotController = GetComponent<ItemSlotController>();
        _headbob = GetComponent<HeadBob>();

        _cameraController.SetFOVLerpSpeed(_lerpSpeed);
    }

    private void Start()
    {
        _itemSlotController.SetSlot(0);
    }

    private void HandleMovement()
    {
        Vector3 moveDir = _inputController.Move;

        if (moveDir == Vector3.zero)
            return;

        float targetSpeed = _defaultMoveSpeed;

        if (_inputController.Sprint)
            targetSpeed *= _sprintMultiplicator;

        _rbMovement.Move(transform.TransformDirection(moveDir), targetSpeed);
    }

    private void HandleJump()
    {
        bool jumpPressed = _inputController.Jump;

        if (!(jumpPressed && !_isJumpQueued))
            return;

        bool isPlayerGrounded = _overlapBoxDetector.CheckForAnyObjects(_groundLayers);

        if (isPlayerGrounded)
            _isJumpQueued = true;
    }

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
    }

    private void HandleHeadbob()
    {
        Vector3 moveDir = _inputController.Move;

        if (moveDir == Vector3.zero)
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

    private void HandleCameraFOV()
    {
        Vector3 moveDir = _inputController.Move;

        if (moveDir == Vector3.zero)
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

    private void HandleItemUse()
    {
        bool wasPrimaryGadgetActionPressed = _inputController.PrimaryGadgetAction.WasPressedThisFrame();
        bool isPrimaryGadgetActionHeldDown = _inputController.PrimaryGadgetAction.IsPressed();

        if (!(wasPrimaryGadgetActionPressed || isPrimaryGadgetActionHeldDown))
            return;

        Transform cameraTransform = _cameraController.GetTransform();

        Usable equippedItem = _itemSlotController.GetEquippedItem();

        if (equippedItem != null)
        {
            if (_inputController)
            equippedItem.Use(
                cameraTransform.position,
                cameraTransform.forward.normalized,
                isPrimaryGadgetActionHeldDown,
                wasPrimaryGadgetActionPressed
            );
        }
        else
        {
            Debug.LogWarning("[PLAYER CONTROLLER] Active item is null! -");
        }
    }

    private void HandleGunReload()
    {
        Usable equippedItem = _itemSlotController.GetEquippedItem();

        if (equippedItem == null)
            return;

        bool wasReloadPressed = _inputController.ReloadWeapon.WasPressedThisFrame();

        if (!wasReloadPressed)
            return;

        if (equippedItem is not Weapon)
            return;

        Weapon weapon = equippedItem as Weapon;
        weapon.Reload();
    }

    private void HandleInventory()
    {
        if (_inputController.ShuffleInventorySlots == 0)
            return;

        int currentIndex = _itemSlotController.CurrentSlot;
        int targetIndex = currentIndex + (int)_inputController.ShuffleInventorySlots;
        _itemSlotController.SetSlot(targetIndex);
    }

    private void Update()
    {
        
        HandleJump();
        HandleCameraLook();
        HandleItemUse();
        HandleGunReload();
        HandleInventory();
        HandleCameraFOV();
        HandleHeadbob();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        if (_isJumpQueued)
        {
            _isJumpQueued = false;
            _rbMovement.Jump(_jumpStrength);
        }
    }
}