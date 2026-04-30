using UnityEngine;
using UnityEngine.UI;

public class BatterySliderDisplay : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderFill;
    [SerializeField] private Color _batteryChargedColor;
    [SerializeField] private Color _batteryDrainedColor;

    public void SetSliderValue(float value)
    {
        _slider.value = value;
        
        if (_sliderFill == null)
            return;
        
        _sliderFill.color = (Color32)Color.Lerp(
            _batteryDrainedColor,
            _batteryChargedColor,
            value / 100f
        );
    } 
}
