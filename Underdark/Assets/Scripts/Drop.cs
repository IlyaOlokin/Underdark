using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    [SerializeField] private DroppedItem droppedItemPref;
    [SerializeField] private List<ItemToDrop> drop;
    
    public void DropItems()
    {
        foreach (var itemToDrop in drop)
        {
            if (Random.Range(0f, 1f) < itemToDrop.ChanceToDrop)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }
        }
    }
}
