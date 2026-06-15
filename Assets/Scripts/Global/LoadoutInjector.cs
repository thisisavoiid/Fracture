using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LoadoutInjector : MonoBehaviour
{
    [SerializeField] private InventoryController _inventory;
    [SerializeField] private InventoryVariable _source;
    public void InjectItems()
    {
        if (_inventory == null || _source == null)
            return;
        
        foreach (Item sourceItem in _source.Value.GetItems())
            _inventory.AddItem(sourceItem);
    }
}
