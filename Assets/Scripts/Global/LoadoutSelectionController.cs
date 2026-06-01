using UnityEngine;

public class LoadoutSelectionController : MonoBehaviour
{
    private Item _selectedItem;
    public Item SelectedItem => _selectedItem;
    public void SelectItem(Item item)
    {
        _selectedItem = item;
    }
}
