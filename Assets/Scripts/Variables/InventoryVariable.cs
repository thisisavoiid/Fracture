using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Variables/New Inventory Variable")]
public class InventoryVariable : ScriptableObject
{
    private List<Usable> _value;
    public List<Usable> Value => _value;
    public void SetValue(List<Usable> value) => _value = value;
}