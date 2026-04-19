using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool", fileName = "Bool Variable")]
public class BoolVariable : ScriptableObject
{
    [SerializeField] private bool _defaultValue;
    private bool _value;
    public bool Value => _value;
    public void SetValue(bool value) => _value = value;
    private void Awake()
    {
        SetValue(_defaultValue);
    }
}