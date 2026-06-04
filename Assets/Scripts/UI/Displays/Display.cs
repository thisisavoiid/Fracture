using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Display<T> : MonoBehaviour 
{
    [SerializeField] 
    [HideIf("_tryGetLabel")] 
    protected TextMeshProUGUI _label;
    [SerializeField] private bool _tryGetLabel = false;

    private void Awake()
    {
        if (!_tryGetLabel)
            return;
        
        _label = GetComponent<TextMeshProUGUI>();
    }

    public void RefreshLabel(T value)
    {
        if (_label == null) 
            return;
        
        _label.text = FormatValue(value);
    }

    protected virtual string FormatValue(T value) => value.ToString();
}