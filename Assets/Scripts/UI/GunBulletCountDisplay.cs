using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TextMeshPro))]
public class GunBulletCountDisplay : MonoBehaviour
{
    [SerializeField] private GunBulletTracker _bulletTracker;
    private TextMeshPro _label;
    private void Awake()
    {
        _label = GetComponent<TextMeshPro>();

        _bulletTracker.OnBulletCountChange.AddListener(
            bulletsRemaining => { RefreshLabelText(bulletsRemaining.ToString()); }
        );
    }

    private void RefreshLabelText(string text)
    {
        _label.text = text;
    }
}
