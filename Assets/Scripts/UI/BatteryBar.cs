using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates a UI Slider to represent battery levels and dynamically changes 
/// the fill color based on the current charge percentage.
/// </summary>
public class BatteryBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderFill;
    [SerializeField] private Color _batteryChargedColor;
    [SerializeField] private Color _batteryDrainedColor;

    /// <summary>
    /// Updates the slider position and interpolates the color between 
    /// drained and charged states.
    /// </summary>
    /// <param name="value">The current battery value (assumed range 0-100).</param>
    public void SetSliderValue(float value)
    {
        _slider.value = value;
        
        if (_sliderFill == null)
            return;
        
        // Linearly interpolates color based on the percentage of the charge
        _sliderFill.color = (Color32)Color.Lerp(
            _batteryDrainedColor,
            _batteryChargedColor,
            value / 100f
        );
    } 
}