using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(InventoryController))]
public class LoadoutInjectorHelper : MonoBehaviour
{
    private InventoryController _inventory;
    [SerializeField] private InventoryVariable _targetInventory;
    [SerializeField] private LoadoutSelectionController _loadoutSelectionController;

    private void Awake()
    {
        _inventory = GetComponent<InventoryController>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void FetchItems()
    {
        if (_loadoutSelectionController == null)
            return;
        
        Item selectedItem = _loadoutSelectionController.SelectedItem;
        _inventory.AddItem(selectedItem);
    }

    [Button("Inject Items")]
    public void InjectItems()
    {
        Debug.Log("[INVENTORY INJECTOR] InjectItems invoked -");

        if (_targetInventory == null)
        {
            Debug.Log("[INVENTORY INJECTOR] Target inventory is null, returning early -");
            return;
        }

        List<Item> items = _inventory.GetItems();

        Debug.Log($"[INVENTORY INJECTOR] Retrieved {items.Count} items from source inventory -");

        foreach (Item item in items)
        {
            Debug.Log($"[INVENTORY INJECTOR] Injecting item '{item.name}' -");

            _targetInventory.Value.AddItem(item);

            Debug.Log($"[INVENTORY INJECTOR] Successfully added item '{item.name}' to target inventory -");
        }

        Debug.Log("[INVENTORY INJECTOR] Item injection finished -");

        Destroy(this.gameObject);
    }
}
