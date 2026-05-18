using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class GameTimeDisplay : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    private TextMeshProUGUI _label;
    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    public void RefreshTimeLabel(TimeMS time)
    {
        if (_label == null)
            return;
        
        _label.text = $"{(int)time.Minutes:D2}:{(int)time.Seconds:D2}";
    }
}
