using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(RigidbodyMovement))]
[RequireComponent(typeof(CameraMovement))]
[RequireComponent(typeof(OverlapBoxDetector))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(ItemSlotController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cameraSensitivity;
    [SerializeField] private float _cameraAngleClamp = 90.0f;
    [SerializeField] private LayerMask _groundLayers;

    [SerializeField] private float _idleHeadbobStrength;
    [SerializeField] private float _walkingHeadbobStrength;

    private PlayerInputController _inputController;
    private ItemSlotController _itemSlotController;
    private RigidbodyMovement _rbMovement;
    private CameraMovement _cameraMovement;
    private OverlapBoxDetector _overlapBoxDetector;
    private Inventory _inventory;
    private Usable _activeItem;
    private bool _isJumpQueued = false;

    private void Awake()
    {
        _inputController = GetComponent<PlayerInputController>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _cameraMovement = GetComponent<CameraMovement>();
        _overlapBoxDetector = GetComponent<OverlapBoxDetector>();
        _inventory = GetComponent<Inventory>();
        _itemSlotController = GetComponent<ItemSlotController>();

        _itemSlotController.OnItemEquipped.AddListener((newItem) => { _activeItem = newItem; });
    }

    private void HandleMovement(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero)
            return;

        _rbMovement.Move(transform.TransformDirection(moveDir));
    }

    private void Update()
    {
        #region Movement

        HandleMovement(_inputController.Move);

        #endregion

        #region Jump

        bool jumpPressed = _inputController.Jump;

        if (jumpPressed && !_isJumpQueued)
        {
            bool isPlayerGrounded = _overlapBoxDetector.CheckForAnyObjects(_groundLayers);

            if (isPlayerGrounded)
                _isJumpQueued = true;
        }

        #endregion

        #region Camera Look (Mouse Movement)

        Vector2 mouseDelta = _inputController.Look;
        float mouseX = mouseDelta.x * _cameraSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * _cameraSensitivity * Time.deltaTime;

        Quaternion currentRbRotation = _rbMovement.GetRotation();
        _rbMovement.SetRotation(Quaternion.Euler(0f, mouseX + currentRbRotation.eulerAngles.y, 0f));

        float pitch = Mathf.DeltaAngle(0f, _cameraMovement.GetLocalRotation().eulerAngles.x);

        pitch += -mouseY;
        pitch = Mathf.Clamp(pitch, -_cameraAngleClamp, _cameraAngleClamp);

        _cameraMovement.SetLocalRotation(Quaternion.Euler(pitch, 0f, 0f));

        #endregion

        #region Shooting / Interaction

        bool wasPrimaryGadgetActionPressed = _inputController.PrimaryGadgetAction.WasPressedThisFrame();
        bool isPrimaryGadgetActionHeldDown = _inputController.PrimaryGadgetAction.IsPressed();

        if (isPrimaryGadgetActionHeldDown)
        {
            Transform cameraTransform = _cameraMovement.GetTransform();

            if (_activeItem != null)
            {
                _activeItem.Use(
                    cameraTransform.position,
                    cameraTransform.forward.normalized,
                    isPrimaryGadgetActionHeldDown,
                    wasPrimaryGadgetActionPressed
                );

                Debug.Log($"[PLAYER CONTROLLER] Using item: {_activeItem.gameObject.name} -");
            }
            else
            {
                Debug.LogWarning("[PLAYER CONTROLLER] Active item is null! -");
            }
        }

        #endregion

        #region Inventory

        if (_inputController.ShuffleInventorySlots != 0)
        {
            int currentIndex = _inventory.GetActiveSlot();
            int targetIndex = currentIndex + (int)_inputController.ShuffleInventorySlots;
            _inventory.SetActiveSlot(targetIndex);

            _activeItem = _itemSlotController.EquipItem(_inventory.GetActiveItem());

            Debug.Log($"[PLAYER CONTROLLER] Shuffled to item: {_inventory.GetActiveItem().gameObject.name} -");
        }

        #endregion
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