using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Variables/New Transform Variable")]
public class TransformVariable : ScriptableObject
{
    private Transform _value;
    public Transform Value => _value;
    public void SetValue(Transform value) => _value = value;
}