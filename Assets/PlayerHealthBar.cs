using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("UI Component Links")]
    [BoxGroup("UI References")]
    [Tooltip("The main health slider that updates immediately when damage is taken.")]
    [SerializeField]
    private Slider _primarySlider;

    [BoxGroup("UI References")]
    [Tooltip("The fill image component of the primary slider used for dynamic color blending.")]
    [SerializeField]
    private Image _primaryFill;

    [BoxGroup("UI References")]
    [Tooltip("The background slider that catches up slowly to visualize the lost health chunk.")]
    [SerializeField]
    private Slider _secondarySlider;

    [Header("Core System Setup")]
    [BoxGroup("Setup")]
    [Tooltip("Reference to the underlying health system data script.")]
    [SerializeField]
    private Health _health;

    [Header("Juice & Feedback Settings")]
    [BoxGroup("Shake Settings")]
    [Tooltip("How long the camera/UI shake effect lasts upon taking damage.")]
    [SerializeField]
    [Range(0f, 3f)]
    private float _shakeDuration = 0.5f;

    [BoxGroup("Shake Settings")]
    [Tooltip("The intensity/strength of the shake effect. Higher values cause more violent movement.")]
    [SerializeField]
    [Range(0f, 50f)]
    private float _shakeIntensity = 12.5f;

    [Header("Bar Tween Timings")]
    [BoxGroup("Bar Animation Timings")]
    [Tooltip("Duration of the primary health bar animation.")]
    [SerializeField]
    [Range(0f, 3f)]
    private float _primaryBarReductionDuration = 1.0f;

    [BoxGroup("Bar Animation Timings")]
    [Tooltip("Delay before the secondary (catch-up) bar starts shrinking.")]
    [SerializeField]
    [Range(0f, 2f)]
    private float _secondaryBarReductionDelay = 0.25f;

    [BoxGroup("Bar Animation Timings")]
    [Tooltip("Duration of the secondary health bar catch-up animation.")]
    [SerializeField]
    [Range(0f, 3f)]
    private float _secondaryBarReductionDuration = 0.2f;

    [Header("Dynamic Color Lerp Settings")]
    [BoxGroup("Color Settings")]
    [Tooltip("Color of the primary health bar when the player is at 100% health.")]
    [SerializeField]
    private Color _fullHealthColor = new Color(0.6f, 1f, 0.6f, 1f);

    [BoxGroup("Color Settings")]
    [Tooltip("Color of the primary health bar when the player is near 0% health.")]
    [SerializeField]
    private Color _lowHealthColor = new Color(1f, 0.4f, 0.4f, 1f);

    [BoxGroup("Color Settings")]
    [Tooltip("How fast the color transitions between full and low health colors.")]
    [SerializeField]
    [Range(0f, 2f)]
    private float _colorFillChangeDuration = 0.75f;

    private Vector3 _primaryBarStartPosition;
    private Vector3 _secondaryBarStartPosition;

    [Button]
    public void ResetTest()
    {
        ClearTweens();
        ResetPositions();
        ResetColors();
    }

    public void PerformHealthBarUpdate()
    {
        ClearTweens();
        ResetPositions();
        PerformTweenShakes();
        PerformTweenValueChanges();
        LerpFillColors();
    }

    private void Awake()
    {
        InitializeSliderValues(
            maxHealth: _health.DefaultHealth,
            useIntegersOnly: false
        );

        SubscribeToHealthEvents();
        SetStartPositionValues();
        ResetColors();
        ResetPositions();
        ClearTweens();
    }

    private void SubscribeToHealthEvents() {
        _health.OnHealthRefresh.AddListener(
            (_) => PerformHealthBarUpdate()
        );
    }

    private void InitializeSliderValues(float maxHealth, bool useIntegersOnly)
    {
        _primarySlider.wholeNumbers = useIntegersOnly;
        _primarySlider.maxValue = maxHealth;
        _primarySlider.minValue = 0.0f;
        _primarySlider.value = maxHealth;
        
        _secondarySlider.wholeNumbers = useIntegersOnly;
        _secondarySlider.maxValue = maxHealth;
        _secondarySlider.minValue = 0.0f;
        _secondarySlider.value = maxHealth;
    }

    private void ResetColors()
    {
        _primaryFill.color = _fullHealthColor;
    }

    private void ResetSliderValues() {
        float maxValue = _primarySlider.maxValue;
        _primarySlider.value = maxValue;
        _secondarySlider.value = maxValue;
    }

    private void SetStartPositionValues()
    {
        _primaryBarStartPosition = _primarySlider.transform.localPosition;
        _secondaryBarStartPosition = _secondarySlider.transform.localPosition;
    }

    private void ClearTweens()
    {
        _primarySlider.DOKill();
        _primarySlider.transform.DOKill();

        _secondarySlider.DOKill();
        _secondarySlider.transform.DOKill();
    }

    private void ResetPositions()
    {
        _primarySlider.transform.localPosition = _primaryBarStartPosition;
        _secondarySlider.transform.localPosition = _secondaryBarStartPosition;
    }

    private void PerformTweenShakes()
    {
        _primarySlider.transform.DOShakePosition(
            duration: _primaryBarReductionDuration,
            strength: _shakeIntensity,
            snapping: true,
            fadeOut: true
        );

        _secondarySlider.transform.DOShakePosition(
            duration: _secondaryBarReductionDuration,
            strength: _shakeIntensity,
            snapping: true,
            fadeOut: true
        ).SetDelay(_secondaryBarReductionDelay);
    }

    private void LerpFillColors()
    {
        Color targetColor = Color.Lerp(
            _lowHealthColor,
            _fullHealthColor,
            _primarySlider.value / _primarySlider.maxValue
        );

        _primaryFill.DOColor(
            targetColor,
            _colorFillChangeDuration
        );
    }

    private void PerformTweenValueChanges()
    {
        float targetValue = _health.CurrentHealth;

        _primarySlider.DOValue(targetValue, _primaryBarReductionDuration);
        _secondarySlider.DOValue(targetValue, _secondaryBarReductionDuration).SetDelay(_secondaryBarReductionDelay);
    }
}
