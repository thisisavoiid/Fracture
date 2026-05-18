using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool _useMainCamera = false;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _baseFov;
    [SerializeField] private float _fovLerpSpeed;
    [SerializeField] private bool _isLocked = false;

    public Camera Camera => _camera;
    private float _targetFov;
    private Transform _cameraTransform;

    private void Awake()
    {
        if (_camera == null)
        {
            Debug.LogError($"[CAMERA MOVEMENT] No camera specified on GameObject: {this.gameObject.name} -");
            return;
        }

        if (_useMainCamera)
            _camera = Camera.main;

        _cameraTransform = _camera.GetComponent<Transform>();
    }

    private void Update()
    {
        if (_camera == null)
            return;

        if (_isLocked)
            return;
            
        float currentFOV = _camera.fieldOfView;

        _camera.fieldOfView = Mathf.Lerp(
            currentFOV,
            _targetFov,
            Time.deltaTime * _fovLerpSpeed
        );
    }

    public void SetTargetFOV(float fov)
    {
        if (_isLocked)
            return;

        _targetFov = fov;
    }
    public void SetFOVLerpSpeed(float speed)
    {
        if (_isLocked)
            return;

        _fovLerpSpeed = speed;
    }

    public Quaternion GetRotation() => _cameraTransform.rotation;

    public void SetRotation(Quaternion rotation)
    {
        if (_isLocked)
            return;

        _cameraTransform.rotation = rotation;
    }

    public Quaternion GetLocalRotation() => _cameraTransform.localRotation;
    public void SetLocalRotation(Quaternion rotation)
    {
        if (_isLocked)
            return;

        _cameraTransform.localRotation = rotation;
    }

    public Vector3 GetPosition() => _cameraTransform.position;

    public void SetPosition(Vector3 pos)
    {
        if (_isLocked)
            return;

        _cameraTransform.position = pos;
    }

    public Vector3 GetLocalPosition() => _cameraTransform.localPosition;
    public void SetLocalPosition(Vector3 pos)
    {
        if (_isLocked)
            return;

        _cameraTransform.localPosition = pos;
    }

    public Transform GetTransform() => _cameraTransform;

    public void SetLocked(bool value) => _isLocked = value;
}