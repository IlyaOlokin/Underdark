using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemsStorage", fileName = "ItemStorage")]
public class ItemsStorageSO : ScriptableObject
{
    [field: SerializeField] public List<Item> Items { get; private set; }

    public void LoadItems()
    {
        Items.Clear();
        var items = Resources.LoadAll<Item>("Items");

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public void Clear()
    {
        Items.Clear();
    }

    public Item GetItemById(string id)
    {
        if (id == "") return null;
        
        foreach (var item in Items)
        {
            if (item.ID == id)
                return item;
        }
        Debug.Log($"No item with ID {id} was found.");

        return null;
    }
}
