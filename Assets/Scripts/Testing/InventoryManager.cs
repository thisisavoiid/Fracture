using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 3;
    public int Capacity => _inventorySize;
    
    [SerializeField][MinValue(0)] private int _defaultSlot = 0;
    [SerializeField] private Transform _itemContainerTransform;
    [SerializeField] private bool _enableDebugMode = false;

    [SerializeField]
    [ReorderableList]
    [ShowIf("_enableDebugMode")]
    [BoxGroup("Debugging")]
    private List<Item> _inventoryContent = new();
    public int ItemCount => _inventoryContent.Count;
    
    [SerializeField]
    [ShowIf("_enableDebugMode")]
    [MinValue(0)]
    private int _targetIndex;

    [SerializeField]
    [ShowIf("_enableDebugMode")]
    [OnValueChanged("RefreshActiveSlot")]
    [MinValue(0)]
    private int _activeSlot = 0;
    public int ActiveSlot => _activeSlot;

    [SerializeField]
    [ShowIf("_enableDebugMode")]
    private Item _activeItem;
    public Item ActiveItem => _activeItem;

    private Dictionary<Item, Item> _itemInstances = new();

    private bool IsActiveItemValid() => _activeItem != null;

    public void AddItem(Item item)
    {
        if (_inventoryContent.Count >= _inventorySize)
        {
            Debug.LogError($"[INVENTORY MANAGER] Cannot add item. Inventory is full (Size: {_inventorySize}) -");
            return;
        }

        InstantiateItem(item, out _);
        _inventoryContent.Add(item);
        int newItemIndex = _inventoryContent.Count - 1;

        if (newItemIndex == _activeSlot)
            RefreshActiveSlot();

        Debug.Log($"[INVENTORY MANAGER] Successfully added item: {item.Config.Name} at index {newItemIndex} -");
    }

    [Button("Clear"), ShowIf("_enableDebugMode")]
    public void Clear()
    {
        int currentItemCount = _inventoryContent.Count;
        _inventoryContent.Clear();

        foreach (Item item in _itemInstances.Keys)
            Destroy(_itemInstances[item].gameObject);

        _itemInstances.Clear();

        Debug.Log($"[INVENTORY MANAGER] Successfully removed {currentItemCount} items from the inventory -");
    }

    [Button("Get Items"), ShowIf("_enableDebugMode")]
    public List<Item> GetItems()
    {
        if (_inventoryContent == null || _inventoryContent.Count == 0)
            return null;

        return _inventoryContent;
    }

    private void RefreshActiveSlot()
    {
        if (!Application.isPlaying)
        {
            _activeSlot = 0;
            return;
        }

        SetActiveSlot(_activeSlot, out _);
    }

    public void SetActiveSlot(int index, out Item item)
    {
        item = null;

        if (index > _inventorySize || index < 0)
        {
            Debug.LogError($"[INVENTORY MANAGER] Failed to set active slot. Provided index ({index}) is outside of valid range -");
            return;
        }

        _activeSlot = index;
        Debug.Log($"[INVENTORY MANAGER] Successfully switched active slot to index: {index} -");
        
        DisableActiveItem();

        if (!IsSlotValid(index))
        {
            _activeItem = null;
            return;
        }
        
        Item itemInstance;

        if (!HasItemAlreadyBeenInstantiated(_inventoryContent[index]))
        {
            InstantiateItem(_inventoryContent[index], out itemInstance);
        }
        else
        {
            itemInstance = _itemInstances[_inventoryContent[index]];
        }

        _activeItem = itemInstance;

        item = _activeItem;

        EnableActiveItem();
    }

    [Button("Disable Active Item"), ShowIf("_enableDebugMode")]
    private void DisableActiveItem()
    {
        if (_activeItem == null)
            return;

        _activeItem.gameObject.SetActive(false);
    }

    [Button("Enable Active Item"), ShowIf("_enableDebugMode")]
    private void EnableActiveItem()
    {
        if (_activeItem == null)
            return;

        _activeItem.gameObject.SetActive(true);
    }

    private void InstantiateItem(Item item, out Item itemInstance)
    {
        Transform targetTransform = this.transform;

        if (_itemContainerTransform != null)
            targetTransform = _itemContainerTransform;

        itemInstance = Instantiate(item, targetTransform);
        itemInstance.gameObject.SetActive(false);

        _itemInstances.Add(item, itemInstance);
    }

    private bool HasItemAlreadyBeenInstantiated(Item item)
    {
        if (item == null)
            return false;

        return _itemInstances.ContainsKey(item);
    }

    private bool IsSlotValid(int index)
    {
        if (index < 0 || index >= _inventoryContent.Count)
            return false;
        
        return _inventoryContent[index] != null;
    }
    
    private void Start()
    {
        SetActiveSlot(_defaultSlot, out _);
    }

    public void UseActiveItem(ItemUsageData usageData)
    {
        if (_activeItem == null)
            return;

        _activeItem.Use(usageData);
    }
}