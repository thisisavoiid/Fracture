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

        _timer.OnTimerUpdate.AddListener(
            timeRemaining =>
            {
                RefreshLabelText(
                    $"{(int)timeRemaining.Minutes:D2}:{(int)timeRemaining.Seconds:D2}"
                );
            }
        );
    }

    private void RefreshLabelText(string text)
    {
        _label.text = text;
    }
}
