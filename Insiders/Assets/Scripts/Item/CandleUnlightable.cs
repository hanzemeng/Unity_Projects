using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleUnlightable : ItemUse
{
    public override bool UseItem(Item item)
    {
        if(null == item)
        {
            return false;
        }
        if(ItemName.ITEM_LIGHTER == item.itemName)
        {
            Hint.hint.ShowHint("the candle is too wet to be ignited by such puny flame.");
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
        }
        return false;
    }
}
