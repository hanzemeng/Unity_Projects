using UnityEngine;

public class CarriableItem : Breakable
{
    [SerializeField] private ItemData itemData;
    public int count = 1;
    private string uniqueId;

    private void Start() { SetID(permanent.id); }

    public void SetID(string newId) {
        if (newId == null || newId == "") { return; }
        uniqueId = newId;
    }

    public void PickUp(GameObject source)
    {
        Neuromancer.NPCUnit npcUnit = source.GetComponent<Neuromancer.NPCUnit>();
        int numberAddedToInventory = 0;
        if (npcUnit != null)
        {
            numberAddedToInventory = npcUnit.inventory.AddItem(itemData, uniqueId, count);
        }

        if (numberAddedToInventory == count)
        {
            Break(false);
            return;
        }
        else if (numberAddedToInventory > 0)
        {
            count -= numberAddedToInventory;
            return;
        }
    }
}
