using UnityEngine;

public abstract class ItemConfiguration : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("The display name of the item used in the UI.")]
    [SerializeField] private string _name;
    public string Name => _name;

    [Header("Sprite")]
    [Tooltip("The sprite displayed for the item in the UI.")]
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    protected GameObject _holder;
}