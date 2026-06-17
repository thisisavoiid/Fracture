using UnityEngine;

public class GunRecoilController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The affected transform component.")]
    [SerializeField] private Transform _gunRecoilTransform;
    private Quaternion _baseRotation;
    private Quaternion _targetRotation;
    private RecoilConfig _recoilConfig;
    private PlayerInputController _playerInputController;

    private void Awake()
    {
        _baseRotation = _gunRecoilTransform.localRotation;
        _targetRotation = _baseRotation;

        _playerInputController = transform.root.GetComponent<PlayerInputController>();
    }

    public void ApplyRecoil(GunContext gunContext)
    {
        if (gunContext.Holder == null)
            return;

        if (gunContext.Holder != transform.root.gameObject)
            return;

        _recoilConfig = gunContext.Gun.RecoilConfig;

        float intensityY = _recoilConfig.IntensityY;
        float minX = _recoilConfig.RandomXRecoilRange.Min;
        float maxX = _recoilConfig.RandomXRecoilRange.Max;

        bool isScoped = _playerInputController != null && _playerInputController.SecondaryGadgetAction.IsPressed();

        if (isScoped)
        {
            intensityY *= _recoilConfig.ScopeRecoilMultiplicator;
            minX *= _recoilConfig.ScopeRecoilMultiplicator;
            maxX *= _recoilConfig.ScopeRecoilMultiplicator;
        }

        Quaternion additionalRotation = Quaternion.Euler(
            -intensityY,
            Random.Range(
                minX,
                maxX
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