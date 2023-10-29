using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightable : ItemUse
{
    public LightSwitch lightSwitchCurrent;
    public LightSwitch lightSwitchSecondFloor;
    public CandleUnlightable candleUnlightable;

    void Start()
    {
        requiredItemName = ItemName.ITEM_LIGHTER;
    }

    public override bool UseItem(Item item)
    {
        if(null == item)
        {
            return false;
        }
        if(item.itemName == requiredItemName)
        {
            Destroy(GetComponent<CandleLightable>());
            item.PlayUseSound();
            lightSwitchCurrent.LightOn();
            lightSwitchSecondFloor.LightOn();
            Destroy(candleUnlightable);
            return true;
        }
        else
        {
            Hint.hint.ShowHint("this item has no effect.");
            return false;
        }
    }
}
