
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Transform _itemSlotTransform;
    private Inventory _inventory;
    private Usable _activeItemSlotItem;
    public UnityEvent<Usable> OnItemEquipped; 

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
        _inventory.OnSlotChange.AddListener(() => { EquipItem(_inventory.GetActiveItem()); });
    }

    private void Start()
    {
        EquipItem(_inventory.GetActiveItem());
    }

    public Usable EquipItem(Usable item)
    {
        Debug.Log($"[ITEM SLOT CONTROLLER] Equipping item: {item.gameObject.name} -");
        UnequipActiveItem();

        item.gameObject.SetActive(true);

        _activeItemSlotItem = item;

        OnItemEquipped?.Invoke(item);

        return item;
    }

    public void UnequipActiveItem()
    {
        if (_activeItemSlotItem == null)
            return;

        Debug.Log($"[ITEM SLOT CONTROLLER] Unequipping item: {_activeItemSlotItem.gameObject.name} -");

        _activeItemSlotItem.gameObject.SetActive(false);
        _activeItemSlotItem = null;

        OnItemEquipped?.Invoke(null);
    }
}
