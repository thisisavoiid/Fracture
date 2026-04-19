using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IItemProvider))]
public class ItemFactory : MonoBehaviour
{
    private List<Usable> _instantiatedItems = new();
    private IItemProvider _itemProvider;
    private void Awake()
    {
        _itemProvider = GetComponent<IItemProvider>();
    }

    public GameObject InstantiateItem(Usable item, bool createAsEnabled)
    {
        if (item == null)
        {
            Debug.LogError("[ITEM FACTORY] An item couldn't be instantiated as it was null -");
            return null;
        }

        Usable createdItemObject = Instantiate(item);
        createdItemObject.gameObject.SetActive(createAsEnabled);
        _instantiatedItems.Add(createdItemObject);
        return createdItemObject.gameObject;
    }

    public GameObject InstantiateItem(Usable item, Transform transform, bool createAsEnabled)
    {
        if (item == null)
        {
            Debug.LogError("[ITEM FACTORY] An item couldn't be instantiated as it was null -");
            return null;
        }
        
        Usable createdItemObject = Instantiate(item, transform, createAsEnabled);
        createdItemObject.gameObject.SetActive(createAsEnabled);
        _instantiatedItems.Add(createdItemObject);
        return createdItemObject.gameObject;
    }

    public List<Usable> GetAllItemInstances() => _instantiatedItems;
}