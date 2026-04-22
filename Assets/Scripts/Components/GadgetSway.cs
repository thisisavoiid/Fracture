using UnityEngine;
using UnityEngine.InputSystem;

public class GadgetSway : MonoBehaviour
{
    [SerializeField] private float _moveSwayStrength;
    [SerializeField] private float _moveSwaySpeed;
    [SerializeField] private float _swaySnappiness;

    [SerializeField] private float _lookSwayStrength;
    private Vector3 _basePosition;
    private Vector3 _baseRotation;
    private Transform _transform;

    private Vector3 _lastMoveValue;
    private Vector2 _lastLookDelta;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _basePosition = _transform.localPosition;
        _baseRotation = _transform.localRotation.eulerAngles;
    }

    public void SetMoveValue(Vector3 move)
    {
        _lastMoveValue = move;   
    }

    public void SetLookValue(Vector2 look)
    {
        _lastLookDelta = look;
    }

    private void Update()
    {
            
        Vector3 targetPosition = _basePosition;
        Vector3 targetRotation = _baseRotation;

        targetRotation.z = _baseRotation.z + _lastMoveValue.x * _moveSwayStrength * -1;
        targetRotation.x = _baseRotation.x + _lastMoveValue.z * _moveSwayStrength ;

        targetRotation.x = targetRotation.x + _lastLookDelta.y * _lookSwayStrength;
        targetRotation.y = _baseRotation.y + _lastLookDelta.x * _lookSwayStrength;

        _transform.localRotation = Quaternion.Lerp(_transform.localRotation, Quaternion.Euler(targetRotation), Time.deltaTime * _moveSwaySpeed);

        // targetPosition.x = _basePosition.x + mouseX * _horizontalSwayStrength;
        // targetPosition.y = _basePosition.y + mouseY * _verticalSwayStrength;

        
        // _transform.localPosition = Vector3.Lerp(_transform.localPosition, targetPosition, _swaySpeed * Time.deltaTime);

        _lastMoveValue = Vector3.Lerp(_lastMoveValue, Vector3.zero, Time.deltaTime * _swaySnappiness);
    }
}