using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    [field: SerializeField] public ItemData itemData { get; private set; }
    [field: SerializeField] public int count { get; private set; } = 0;
    [field: SerializeField] public string uniqueId { get; private set; } = null;

    public InventoryItem(ItemData itemData, string id)
    {
        this.itemData = itemData;
        count = 0;
        uniqueId = id;
    }

    public int AddToStack(int c = 1)
    {
        if (count < itemData.maxStackSize)
        {
            count += c;
            if (count > itemData.maxStackSize)
            {
                int diff = count - itemData.maxStackSize;
                count = itemData.maxStackSize;
                return c - diff;
            }
            return c;
        }
        return 0;
    }

    public int RemoveFromStack(int c = 1)
    {
        if (count > 0)
        {
            count -= c;
            if (count < 0)
            {
                int diff = -count;
                count = 0;
                return c - diff;
            }
            return c;
        }
        return 0;
    }

}
