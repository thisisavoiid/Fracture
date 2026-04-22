using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TextMeshPro))]
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health _health;
    private TextMeshPro _label;
    private void Awake()
    {
        _label = GetComponent<TextMeshPro>();

        _health.OnHealthRefresh.AddListener(
            healthRemaining => { RefreshLabelText(healthRemaining.ToString()); }
        );
    }

    private void Start()
    {
        RefreshLabelText(_health.CurrentHealth.ToString());
    }
    
    private void RefreshLabelText(string text)
    {
        _label.text = text;
    }
}
