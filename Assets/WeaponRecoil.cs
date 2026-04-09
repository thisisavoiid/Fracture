using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private Transform _transform;

    private Quaternion _baseRotation;
    private Quaternion _targetRotation;

    [SerializeField] private float _recoilStrength;
    [SerializeField] private float _returnSpeed;
    [SerializeField] private float _snappiness;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _baseRotation = _transform.localRotation; // Set base rotation to have a default value to return to
    }

    public void ApplyRecoil()
    {
        _targetRotation *= Quaternion.Euler(_recoilStrength, 0f, 0f); // "Adding" rotation values onto eachother
    }

    private void Update()
    {
        _targetRotation = Quaternion.Slerp(_targetRotation, _baseRotation, _returnSpeed * Time.deltaTime); // Constant force to get back to the default rotation
        _transform.localRotation = Quaternion.Slerp(_transform.localRotation, _targetRotation, _snappiness * Time.deltaTime); // Snappiness = How fast to reach the target rotation
    }
}
