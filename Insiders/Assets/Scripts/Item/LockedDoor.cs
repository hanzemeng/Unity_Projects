using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : ItemUse
{
    void Start()
    {
        requiredItemName = ItemName.ITEM_BASEMENT_KEY;
    }

    public override bool UseItem(Item item)
    {
        if(null == item)
        {
            return false;
        }
        if(item.itemName == requiredItemName)
        {
            Destroy(GetComponent<LockedDoor>());
            item.PlayUseSound();
            return true;
        }
        else if(item.itemName == ItemName.ITEM_AXE)
        {
            Hint.hint.ShowHint("the axe is not durable enough to break through a rigid door like this.");
            return false;
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
            return false;
        }
    }
}
