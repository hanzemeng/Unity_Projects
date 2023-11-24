using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Inventory
{
    [field: SerializeField] public List<InventoryItem> storage { get; private set; }
    [field: SerializeField] public int maxStorageSize { get; private set; }

    private Dictionary<ItemData, InventoryItem> itemLookupDictionary;
    [HideInInspector] public UnityEvent OnInventoryChangeEvent = new UnityEvent();

    public Inventory(int maxSize = 1)
    {
        maxStorageSize = maxSize;
        storage = new List<InventoryItem>(maxStorageSize);
        itemLookupDictionary = new Dictionary<ItemData, InventoryItem>();
    }

    public int AddItem(ItemData itemData, string id, int count = 1)
    {
        if (count < 1)
            return 0;

        // Item is already in inventory (this implementation does not allow 2 stacks of the same item, but using a dictionary is more efficient)
        if (itemLookupDictionary.TryGetValue(itemData, out InventoryItem item)) {
            int numberAdded = item.AddToStack(count);
            OnInventoryChangeEvent.Invoke();
            return numberAdded;
        }

        // No more space in storage
        if (storage.Count >= maxStorageSize)
            return 0;

        // Add New Item to storage
        InventoryItem newItem = new InventoryItem(itemData, id);
        storage.Add(newItem);
        itemLookupDictionary.Add(itemData, newItem);

        int result = newItem.AddToStack(count);
        OnInventoryChangeEvent.Invoke();
        return result;
    }

    public int RemoveItem(ItemData itemData, int count = 1)
    {
        if (count < 1)
            return 0;

        // Item is already in inventory
        int numberRemoved = 0;
        if (itemLookupDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            numberRemoved = item.RemoveFromStack(count);
            if (numberRemoved == 0) // If not successful at removing, return immediately
            {
                return numberRemoved;
            }

            // Check if item stack is empty
            if (item.count == 0)
            {
                storage.Remove(item);
                itemLookupDictionary.Remove(itemData);
                OnInventoryChangeEvent.Invoke();
                return numberRemoved;
            }
        }
        OnInventoryChangeEvent.Invoke();
        return numberRemoved;
    }

    public int CountInStorage(ItemData itemData)
    {
        if (itemLookupDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            return item.count;
        }
        return 0;
    }
}
