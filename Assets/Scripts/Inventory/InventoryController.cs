using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InventoryController : MonoBehaviour, IItemProvider
{
    [SerializeField] private List<Item> _inventoryContent;
    [SerializeField] private InventoryVariable _inventoryVariable;

    private void Awake()
    {
        RefreshInventoryVariable();
    }
    
    private void RefreshInventoryVariable()
    {
        if (_inventoryVariable == null)
            return;

        _inventoryVariable.SetValue(_inventoryContent);
    }

    public void AddItem(Item item)
    {
        AddItem(item, _inventoryContent.Count);

        RefreshInventoryVariable();
    }

    public void AddItem(Item item, int index)
    {
        if (!IsSlotUnoccupied(index))
            return;

        _inventoryContent[index] = item;

        RefreshInventoryVariable();
    }

    public List<Item> GetItems() => _inventoryContent;
    private bool IsSlotUnoccupied(int index) => _inventoryContent[index] == null;

}