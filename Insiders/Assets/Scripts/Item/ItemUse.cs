using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemUse : MonoBehaviour
{
    protected string requiredItemName;
    public string messageBeforeItemUsed;
    public string messageAfterItemUsed;
    protected bool itemUsed;

    void Start()
    {
        itemUsed = false;
    }

    public abstract bool UseItem(Item item);

    public string GetDisplayMessage()
    {
        return itemUsed ? messageAfterItemUsed : messageBeforeItemUsed;
    }
}
