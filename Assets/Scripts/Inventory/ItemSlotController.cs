
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

    public Usable EquipItem(Usable item)
    {
        UnequipActiveItem();

        Usable handheldItem = Instantiate(item, _itemSlotTransform);
        handheldItem.gameObject.name = item.name;
        
        handheldItem.transform.localPosition = Vector3.zero;
        _activeItemSlotItem = handheldItem;

        OnItemEquipped?.Invoke(handheldItem);

        return handheldItem;
    }

    public void UnequipActiveItem()
    {
        if (_activeItemSlotItem == null)
            return;

        Destroy(_activeItemSlotItem.gameObject);

        _activeItemSlotItem = null;

        OnItemEquipped?.Invoke(null);
    }
}
