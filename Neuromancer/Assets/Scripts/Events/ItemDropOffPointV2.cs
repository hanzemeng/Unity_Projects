using UnityEngine;
using System;

public class ItemDropOffPointV2 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dropOffFX;
    [SerializeField] private Mode mode = Mode.CONSUME;
    [SerializeField] private int batchSize = 1;

    [SerializeField] private int numberToSatisfy = 1;

    public ItemData[] possibleItems; // Length of 0 means take everything

    [SerializeField] int id;
    [SerializeField] GameEvent OnDropOffPointSatisfied;

    public enum Mode
    {
        CONSUME = 0,
        PLACE_DOWN,
    }

    public void DropOff(GameObject source)
    {
        

        Neuromancer.NPCUnit npcUnit = source.GetComponent<Neuromancer.NPCUnit>();
        if (npcUnit == null)
        {
            return;
        }

        if (npcUnit.inventory.storage.Count > 0)
        {
            InventoryItem item = npcUnit.inventory.storage[0];
            HandleModes(npcUnit, item);
        }

    }

    private void HandleModes(Neuromancer.NPCUnit npcUnit, InventoryItem item)
    {
        int count = npcUnit.inventory.CountInStorage(item.itemData);
        if (count > batchSize)
            count = batchSize;

        if (count > 0)
        {
            if (mode == Mode.CONSUME)
            {
                ConsumeItem(npcUnit, item, count);
                if(dropOffFX = null)
                {
                    Instantiate(dropOffFX, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                }
            }
            else if (mode == Mode.PLACE_DOWN)
            {
                PlaceDownItem(npcUnit, item.itemData, count);
            }
        }
    }

    private void ConsumeItem(Neuromancer.NPCUnit npcUnit, InventoryItem item, int count)
    {
        if (!item.itemData.useable || !Array.Exists(possibleItems, element => element == item.itemData))
        {
            Debug.Log("Item is not useable/consumeable");
            return;
        }

        if (count > numberToSatisfy)
            count = numberToSatisfy;

        
        int numberRemoved = npcUnit.inventory.RemoveItem(item.itemData, count);
        numberToSatisfy -= numberRemoved;
        ObjectPermanent.SetDestroyID(item.uniqueId);

        if (numberToSatisfy == 0)
        {
            OnDropOffPointSatisfied?.Invoke(id); // Listen to this event to know when the dropoff point is satisfied
        }
    }

    private void PlaceDownItem(Neuromancer.NPCUnit npcUnit, ItemData itemData, int count)
    {
        if (!itemData.placeable)
        {
            Debug.Log("Item is not placeable");
            return;
        }

        if (count > numberToSatisfy)
            count = numberToSatisfy;

        int numberRemoved = npcUnit.inventory.RemoveItem(itemData, count);
        numberToSatisfy -= numberRemoved;
        GameObject newObject = Instantiate(itemData.itemPrefab, transform.position, Quaternion.identity);
        CarriableItem carriableItem = newObject.GetComponent<CarriableItem>();
        carriableItem.count = numberRemoved;

        if (numberToSatisfy == 0)
        {
            OnDropOffPointSatisfied?.Invoke(id); // Listen to this event to know when the dropoff point is satisfied
        }
    }
}
