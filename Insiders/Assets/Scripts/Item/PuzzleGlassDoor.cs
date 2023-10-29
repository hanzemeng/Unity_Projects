using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGlassDoor : ItemUse
{
    public override bool UseItem(Item item)
    {
        if(null == item)
        {
            return false;
        }
        if(item.itemName == ItemName.ITEM_BASEMENT_KEY)
        {
            Hint.hint.ShowHint("there is no keyhole.");
        }
        else if(item.itemName == ItemName.ITEM_AXE)
        {
            Hint.hint.ShowHint("you don’t want the shattered glasses to cut you open. ");
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
        }
        return false;
    }
}
