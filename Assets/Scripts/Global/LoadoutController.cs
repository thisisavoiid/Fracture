using UnityEngine;

[RequireComponent(typeof(InventoryController))]
public class LoadoutController : MonoBehaviour
{
    [SerializeField] private InventoryVariable _storage;
    private InventoryController _inventory;

    private void Awake()
    {
        _inventory = GetComponent<InventoryController>();
        _storage.SetValue(_inventory);
    }

    public void AddItem(Item item)
    {
        _inventory.Clear();
        _inventory.AddItem(item);
    }
}
