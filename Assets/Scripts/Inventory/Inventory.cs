using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Usable> _inventoryContent;
    public UnityEvent OnSlotChange;
    private int _currentSlotIndex = 0;

    private bool IsIndexAvailable(int index) => index >= 0 && index < _inventoryContent.Count;
    private bool IsIndexOccupied(int index) => _inventoryContent[index] != null;

    private void Start()
    {
        SetActiveSlot(0);
    }

    public void SetActiveSlot(int index)
    {
        if (index < 0)
        {
            _currentSlotIndex = _inventoryContent.Count - 1;
            OnSlotChange.Invoke();
            return;
        }

        if (index > _inventoryContent.Count - 1)
        {
            _currentSlotIndex = 0;
            OnSlotChange.Invoke();
            return;
        }

        _currentSlotIndex = index;
        OnSlotChange.Invoke();

        Debug.Log($"[INVENTORY CONTROLLER] Inventory slot set to: {index} -");
    }

    public void RemoveItem(int index)
    {
        _inventoryContent.RemoveAt(index);
        Debug.Log($"[INVENTORY CONTROLLER] Inventory item removed at index: {index} -");
    }

    public void AddItem(Usable item, int index)
    {
        if (!IsIndexAvailable(index))
        {
            Debug.LogError($"[INVENTORY CONTROLLER] Inventory slot index of {index} on {gameObject.name} is out of bounds -");
            return;
        }

        if (IsIndexOccupied(index))
            RemoveItem(index);

        _inventoryContent[index] = item;
        Debug.Log($"[INVENTORY CONTROLLER] Inventory item added at index: {index} -");
    }

    public int GetActiveSlot() => _currentSlotIndex;
    public Usable GetActiveItem()
    {
        int activeSlot = GetActiveSlot();
        if (!IsIndexOccupied(activeSlot))
        {
            Debug.LogWarning($"[INVENTORY CONTROLLER] Inventory slot index of {activeSlot} on {gameObject.name} has no item to retrieve -");
            return null;
        }

        return _inventoryContent[activeSlot];
    }
}