using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private Transform _transform;

    private Vector3 _basePosition;
    private Vector3 _targetPosition;

    [SerializeField] private float _recoilStrength;
    [SerializeField] private float _returnSpeed;
    [SerializeField] private float _snappiness;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _basePosition = _transform.localPosition; // Set base rotation to have a default value to return to
    }

    public void ApplyRecoil()
    {
        _targetPosition.z += _recoilStrength;
    }

    private void Update()
    {
        _targetPosition = Vector3.Lerp(_targetPosition, _basePosition, _returnSpeed * Time.deltaTime); // Constant force to get back to the default rotation
        _transform.localPosition = Vector3.Lerp(_transform.localPosition, _targetPosition, _snappiness * Time.deltaTime); // Snappiness = How fast to reach the target rotation
    }
}
