using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InventoryController : MonoBehaviour, IItemProvider
{
    [SerializeField] private List<Usable> _inventoryContent;
    [SerializeField] private InventoryVariable _inventoryVariable;

    private void Awake()
    {
        RefreshInventoryVariable();
    }
    
    private void RefreshInventoryVariable()
    {
        if (_inventoryVariable == null)
            return;

        Debug.Log("refreshing list...");
        Debug.Log(_inventoryContent.Count);
        _inventoryVariable.SetValue(_inventoryContent);
        Debug.Log(_inventoryVariable.Value.Count);
    }

    public void AddItem(Usable item)
    {
        AddItem(item, _inventoryContent.Count);

        RefreshInventoryVariable();
    }

    public void AddItem(Usable item, int index)
    {
        if (!IsSlotUnoccupied(index))
            return;

        _inventoryContent[index] = item;

        RefreshInventoryVariable();
    }

    public List<Usable> GetItems() => _inventoryContent;
    private bool IsSlotUnoccupied(int index) => _inventoryContent[index] == null;

}