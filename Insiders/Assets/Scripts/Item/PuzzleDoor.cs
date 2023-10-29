using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDoor : ItemUse
{
    public override bool UseItem(Item item)
    {
        if(null == item)
        {
            return false;
        }
        if(item.itemName == ItemName.ITEM_BASEMENT_KEY)
        {
            Hint.hint.ShowHint("the key does not fit.");
        }
        else if(item.itemName == ItemName.ITEM_AXE)
        {
            Hint.hint.ShowHint("the axe is not durable enough to break through a rigid door like this.");
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
        }
        return false;
    }
}
