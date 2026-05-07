using UnityEngine;

public class GunRecoilController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The affected transform component.")]
    [SerializeField] private Transform _gunRecoilTransform;
    private Quaternion _baseRotation;
    private Quaternion _targetRotation;
    private RecoilConfig _recoilConfig;

    private void Awake()
    {
        _baseRotation = _gunRecoilTransform.localRotation;
        _targetRotation = _baseRotation;
    }

    public void ApplyRecoil(GunConfig config)
    {
        _recoilConfig = config.RecoilConfig;

        Quaternion additionalRotation = Quaternion.Euler(
            -_recoilConfig.IntensityY,
            Random.Range(
                _recoilConfig.RandomXRecoilRange.Min,
                _recoilConfig.RandomXRecoilRange.Max
            ),
            0f
        );

        _targetRotation *= additionalRotation;
    }

    private void Update()
    {
        _targetRotation = Quaternion.Lerp(_targetRotation, _baseRotation, Time.deltaTime * _recoilConfig.ForceBackToDefault);

        Quaternion currRotation = _gunRecoilTransform.localRotation;
        _gunRecoilTransform.localRotation = Quaternion.Lerp(currRotation, _targetRotation, Time.deltaTime * _recoilConfig.ForceToTargetForce);
    }
}