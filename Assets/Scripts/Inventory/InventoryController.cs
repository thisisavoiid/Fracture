using System.Collections.Generic;
using UnityEngine;
public class InventoryController : MonoBehaviour, IItemProvider
{
    [SerializeField] private List<Usable> _inventoryContent;

    public void AddItem(Usable item)
    {
        AddItem(item, _inventoryContent.Count);
    }

    public void AddItem(Usable item, int index)
    {
        if (!IsSlotUnoccupied(index))
            return;
        
        _inventoryContent[index] = item;
    }
    
    public List<Usable> GetItems() => _inventoryContent;
    private bool IsSlotUnoccupied(int index) => _inventoryContent[index] == null;
}