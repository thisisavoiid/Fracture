using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(RigidbodyMovement))]
[RequireComponent(typeof(CameraMovement))]
[RequireComponent(typeof(OverlapBoxDetector))]
[RequireComponent(typeof(HeadBob))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cameraSensitivity;
    [SerializeField] private float _cameraAngleClamp = 90.0f;
    [SerializeField] private LayerMask _groundLayers;

    [SerializeField] private GunController _gun; // Testing! Change this to a loadout system later!
    [SerializeField] private float _idleHeadbobStrength;
    [SerializeField] private float _walkingHeadbobStrength;

    private PlayerInputController _inputController;
    private RigidbodyMovement _rbMovement;
    private CameraMovement _cameraMovement;
    private OverlapBoxDetector _overlapBoxDetector;
    private HeadBob _headBob;
    private bool _isJumpQueued = false;

    private void Awake()
    {
        _inputController = GetComponent<PlayerInputController>();
        _rbMovement = GetComponent<RigidbodyMovement>();
        _cameraMovement = GetComponent<CameraMovement>();
        _overlapBoxDetector = GetComponent<OverlapBoxDetector>();
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

        if (_inputController.PrimaryGadgetAction)
        {
            Transform cameraTransform = _cameraMovement.GetTransform();

            _gun.Shoot(
                cameraTransform.position,
                cameraTransform.forward.normalized
            );
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