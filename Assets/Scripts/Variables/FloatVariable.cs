using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Variables/New Float Variable")]
public class FloatVariable : ScriptableObject
{
    private float _value;
    public float Value => _value;
    public void SetValue(float value) => _value = value;
}