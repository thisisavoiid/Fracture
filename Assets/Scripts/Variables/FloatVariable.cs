using UnityEngine;

/// <summary>
/// A ScriptableObject-based variable that allows for modular, decoupled float storage 
/// across different systems and scenes.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Variables/New Float Variable")]
public class FloatVariable : ScriptableObject
{
    private float _value;

    /// <summary>
    /// The current value of the variable.
    /// </summary>
    public float Value => _value;

    /// <summary>
    /// Sets a new value for the variable.
    /// </summary>
    /// <param name="value">The float value to store.</param>
    public void SetValue(float value) => _value = value;
}