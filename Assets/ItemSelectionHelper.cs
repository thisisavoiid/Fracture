using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InventoryController))]
public class ItemSelectionHelper : MonoBehaviour
{
    [SerializeField] private UnityEvent<Item> _onItemSelected;
    [SerializeField] private InventoryVariable _playerInventory;
    private InventoryController _inventory;

    private void Awake()
    {
        _inventory = GetComponent<InventoryController>();
    }

    public void SelectItem(Item item)
    {
        _inventory.Clear();
        _inventory.AddItem(item);
        _onItemSelected?.Invoke(item);
    }
}
