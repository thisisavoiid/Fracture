using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(RigidbodyMovement))]
[RequireComponent(typeof(CameraMovement))]
[RequireComponent(typeof(OverlapBoxDetector))]
[RequireComponent(typeof(ItemSlotController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cameraSensitivity;
    [SerializeField] private float _cameraAngleClamp = 90.0f;
    [SerializeField] private LayerMask _groundLayers;

    [SerializeField] private float _idleHeadbobStrength;
    [SerializeField] private float _walkingHeadbobStrength;

    private PlayerInputController _inputController;
    private RigidbodyMovement _rbMovement;
    private CameraMovement _cameraMovement;
    private OverlapBoxDetector _overlapBoxDetector;
    private ItemSlotController _itemSlotController;
    private bool _isJumpQueued = false;

    private void Awake()
    {
        _inputController = GetComponent<PlayerInputController>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _cameraMovement = GetComponent<CameraMovement>();
        _overlapBoxDetector = GetComponent<OverlapBoxDetector>();
        _itemSlotController = GetComponent<ItemSlotController>();
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

        _rbMovement.Move(transform.TransformDirection(moveDir));
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

        float pitch = Mathf.DeltaAngle(0f, _cameraMovement.GetLocalRotation().eulerAngles.x);

        pitch += -mouseY;
        pitch = Mathf.Clamp(pitch, -_cameraAngleClamp, _cameraAngleClamp);

        _cameraMovement.SetLocalRotation(Quaternion.Euler(pitch, 0f, 0f));
    }

    private void HandleItemUse()
    {
        bool wasPrimaryGadgetActionPressed = _inputController.PrimaryGadgetAction.WasPressedThisFrame();
        bool isPrimaryGadgetActionHeldDown = _inputController.PrimaryGadgetAction.IsPressed();

        if (!(wasPrimaryGadgetActionPressed || isPrimaryGadgetActionHeldDown))
            return;

        Transform cameraTransform = _cameraMovement.GetTransform();

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
        HandleMovement();
        HandleJump();
        HandleCameraLook();
        HandleItemUse();
        HandleGunReload();
        HandleInventory();
    }

    private void FixedUpdate()
    {
        if (_isJumpQueued)
        {
            _isJumpQueued = false;
            _rbMovement.Jump();
        }
    }
}