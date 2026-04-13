using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Usable> _inventoryContent;
    [SerializeField] private Transform _itemSlotTransform;
    public UnityEvent<Usable> OnItemEquipped;
    public UnityEvent OnSlotChange;
    private List<Usable> _inventoryContentInstantiated = new();
    private int _currentSlotIndex = 0;

    private bool IsIndexAvailable(int index) => index >= 0 && index < _inventoryContent.Count;
    private bool IsIndexOccupied(int index) => _inventoryContent[index] != null;

    private void Awake()
    {
        Usable[] inventoryContent = GetInventoryContent();

        for (int i = 0; i < inventoryContent.Count(); i++)
        {
            Debug.Log($"[INVENTORY] Instantiating item at index {i} -");

            Usable newItem = Instantiate(inventoryContent[i], _itemSlotTransform);

            if (newItem == null)
                Debug.LogWarning($"[INVENTORY] Instantiated item at index {i} is NULL -");

            _inventoryContentInstantiated.Add(newItem);

            AddItem(
                newItem,
                i
            );

            newItem.gameObject.SetActive(false);
        }

    }

    private void Start()
    {
        SetActiveSlot(0);
    }

    public Usable[] GetInventoryContent()
    {
        return _inventoryContent.ToArray();
    }

    public void SetActiveSlot(int index)
    {            
        if (index < 0)
        {
            _currentSlotIndex = _inventoryContent.Count - 1;
            OnSlotChange.Invoke();
            Debug.Log($"[INVENTORY] Inventory slot set to: {_currentSlotIndex} -");
            return;
        }

        if (index > _inventoryContent.Count - 1)
        {
            _currentSlotIndex = 0;
            OnSlotChange.Invoke();
            Debug.Log($"[INVENTORY] Inventory slot set to: {_currentSlotIndex} -");
            return;
        }

        _currentSlotIndex = index;
        OnSlotChange.Invoke();
        Debug.Log($"[INVENTORY] Inventory slot set to: {_currentSlotIndex} -");
    }

    public void RemoveItem(int index)
    {
        _inventoryContent.RemoveAt(index);
        Debug.Log($"[INVENTORY] Inventory item removed at index: {index} -");
    }

    public void AddItem(Usable item, int index)
    {
        if (!IsIndexAvailable(index))
        {
            Debug.LogError($"[INVENTORY] Inventory slot index of {index} on {gameObject.name} is out of bounds -");
            return;
        }

        if (IsIndexOccupied(index))
            Debug.LogWarning($"[INVENTORY] Slot {index} was already occupied and has been overwritten -");

        _inventoryContent[index] = item;
        Debug.Log($"[INVENTORY] Inventory item added at index: {index} -");
    }

    public int GetActiveSlot() => _currentSlotIndex;
    public Usable GetActiveItem()
    {
        int activeSlot = GetActiveSlot();
        if (!IsIndexOccupied(activeSlot))
        {
            Debug.LogWarning($"[INVENTORY] Inventory slot index of {activeSlot} on {gameObject.name} has no item to retrieve -");
            return null;
        }

        return _inventoryContent[activeSlot];
    }
}