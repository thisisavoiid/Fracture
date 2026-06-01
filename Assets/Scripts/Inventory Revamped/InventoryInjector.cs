using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class InventoryInjector : MonoBehaviour
{
    [SerializeField] private InventoryController _inventory;
    [SerializeField] private Item _itemToAdd;
    [SerializeField] private List<KeyItemPair> _itemTriggers = new();

    [Button("Get Inventory Content")]
    private void GetInventoryContent()
    {
        List<Item> receivedItems = _inventory.GetItems();

        if (receivedItems == null)
        {
            Debug.Log($"No items found!");
            return;
        }

        foreach (Item item in receivedItems)
        {
            Debug.Log($"[{item.Config.Name}] - Has Sprite: {item.Config.Sprite != null} -");
        }
    }

    [Button("Add Item"), ShowIf("IsItemToAddSpecified")]
    private void AddItemToInventory()
    {
        if (_itemToAdd == null)
            return;

        _inventory.AddItem(_itemToAdd);
        _itemToAdd = null;
    }

    [Button("Clear All Items")]
    private void ClearInventory()
    {
        _inventory.Clear();
    }
    
    private bool IsItemToAddSpecified() => _itemToAdd != null;

    private void Update()
    {
        if (_itemTriggers == null)
            return;
        
        if (_itemTriggers.Count == 0)
            return;
        
        foreach (var pair in _itemTriggers)
        {
            if (Input.GetKeyDown(pair.KeyCode))
                _inventory.AddItem(pair.Item);
        }
    }
}