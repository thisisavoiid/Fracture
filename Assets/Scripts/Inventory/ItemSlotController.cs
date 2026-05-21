
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(IItemProvider))]
[RequireComponent(typeof(ItemFactory))]
public class ItemSlotController : MonoBehaviour
{
    [SerializeField] private Transform _itemSlotTransform;
    [SerializeField] private int _defaultSlot = 0;
    private IItemProvider _itemProvider;
    private ItemFactory _itemFactory;
    private int _currentSlot;
    public int CurrentSlot => _currentSlot;
    public UnityEvent<Item> OnItemEquipped;

    private void Awake()
    {
        _itemProvider = GetComponent<IItemProvider>();
        _itemFactory = GetComponent<ItemFactory>();

        List<Item> items = _itemProvider.GetItems();

        foreach (Item item in items)
            _itemFactory.InstantiateItem(item, _itemSlotTransform, false);
    }

    private void Start()
    {
        List<Item> items = _itemFactory.GetAllItemInstances();

        if (_defaultSlot < 0 || _defaultSlot > items.Count - 1)
            return;

        if (items[_defaultSlot] == null)
            return;

        SetSlot(_defaultSlot);
    }
    public void SetSlot(int index)
    {
        List<Item> itemInstances = _itemFactory.GetAllItemInstances();

        if (itemInstances.Count == 0)
            return;
            
        if (index < 0)
            index = itemInstances.Count - 1;

        if (index > itemInstances.Count - 1)
            index = 0;

        Item currentItem = itemInstances[_currentSlot];

        if (currentItem != null)
            UnequipItem(currentItem);

        _currentSlot = index;

        EquipItem(_itemFactory.GetAllItemInstances()[_currentSlot]);
    }

    private void EquipItem(Item item)
    {
        if (item.gameObject == null)
            return;

        item.gameObject.SetActive(true);
        OnItemEquipped.Invoke(item);
    
        Debug.Log($"[ITEM SLOT CONTROLLER] Equipped now: {item.gameObject.name} -");
    }

    private void UnequipItem(Item item)
    {
        if (item.gameObject == null)
            return;

        item.gameObject.SetActive(false);
        OnItemEquipped.Invoke(null);

        Debug.Log($"[ITEM SLOT CONTROLLER] Unequipped item: {item.gameObject.name} -");
    }

    public Item GetEquippedItem()
    {
        List<Item> items = _itemFactory.GetAllItemInstances();

        if (_currentSlot < 0 || _currentSlot > items.Count - 1)
            return null;

        Item item = items[_currentSlot];
        return item;
    }
}
