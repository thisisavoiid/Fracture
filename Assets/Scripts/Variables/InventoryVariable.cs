using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A ScriptableObject-based variable that stores a list of Usable items, 
/// facilitating a shared inventory state across different game systems.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Variables/New Inventory Variable")]
public class InventoryVariable : ScriptableObject
{
    private InventoryController _inventory;

    /// <summary>
    /// Returns the current list of Usable items in the inventory.
    /// </summary>
    public InventoryController Value => _inventory;

    /// <summary>
    /// Updates the inventory with a new list of Usable items.
    /// </summary>
    /// <param name="value">The new list of items to be stored.</param>
    public void SetValue(InventoryController value) => _inventory = value;
}