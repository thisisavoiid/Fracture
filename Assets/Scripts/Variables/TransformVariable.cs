using UnityEngine;

/// <summary>
/// A ScriptableObject-based variable that stores a Transform reference, 
/// allowing different systems to access a specific object's position, rotation, and scale.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Variables/New Transform Variable")]
public class TransformVariable : ScriptableObject
{
    private Transform _value;

    /// <summary>
    /// The current Transform reference stored in this variable.
    /// </summary>
    public Transform Value => _value;

    /// <summary>
    /// Sets a new Transform reference.
    /// </summary>
    /// <param name="value">The Transform to be stored.</param>
    public void SetValue(Transform value) => _value = value;
}