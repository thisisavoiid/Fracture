using UnityEditor;
using UnityEngine;

public class GadgetSway : MonoBehaviour
{
    [SerializeField] private float _horizontalSwayStrength;
    [SerializeField] private float _verticalSwayStrength;
    [SerializeField] private float _swaySpeed;
    [SerializeField] private float _horizontalRotationStrength;
    [SerializeField] private float _verticalRotationStrength;
    [SerializeField] private float _rotateSpeed;
    private Vector3 _basePosition;
    private Vector3 _baseRotation;
    private Transform _transform;
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _basePosition = _transform.localPosition;
        _baseRotation = _transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        Vector3 targetPosition = _basePosition;
        Vector3 targetRotation = _baseRotation;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        targetPosition.x = _basePosition.x + mouseX * _horizontalSwayStrength;
        targetPosition.y = _basePosition.y + mouseY * _verticalSwayStrength;

        targetRotation.y = _baseRotation.y + mouseX * _horizontalRotationStrength;
        targetRotation.x = _baseRotation.x + mouseY * _verticalRotationStrength;

        _transform.localPosition = Vector3.Lerp(_transform.localPosition, targetPosition, _swaySpeed * Time.deltaTime);
        _transform.localRotation = Quaternion.Lerp(_transform.localRotation, Quaternion.Euler(targetRotation), Time.deltaTime * _rotateSpeed);
    }
}