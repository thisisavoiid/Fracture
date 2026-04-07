using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool _useMainCamera = false;
    [SerializeField] private Camera _camera;
    public Camera Camera => _camera;

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

    public Quaternion GetRotation() => _cameraTransform.rotation;

    public void SetRotation(Quaternion rotation)
    {
        _cameraTransform.rotation = rotation;
    }

    public Quaternion GetLocalRotation() => _cameraTransform.localRotation;
    public void SetLocalRotation(Quaternion rotation)
    {
        _cameraTransform.localRotation = rotation;
    }

    public Vector3 GetPosition() => _cameraTransform.position;

    public void SetPosition(Vector3 pos)
    {
        _cameraTransform.position = pos;
    }
    
    public Vector3 GetLocalPosition() => _cameraTransform.localPosition;
    public void SetLocalPosition(Vector3 pos)
    {
        _cameraTransform.localPosition = pos;
    }

    public Transform GetTransform() => _cameraTransform;
}