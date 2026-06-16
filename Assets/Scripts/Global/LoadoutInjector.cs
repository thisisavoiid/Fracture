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

        List<Item> items = _source.Value.GetItems();

        if (items == null || items.Count == 0)
            return;

        foreach (Item sourceItem in _source.Value.GetItems())
        {
            if (sourceItem == null)
                continue;

            _inventory.AddItem(sourceItem);
        }

    }
}
