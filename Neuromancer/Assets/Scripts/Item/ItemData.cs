using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public GameObject itemPrefab;

    [Tooltip("Item that can be placed on the ground. Ex: Rock")]
    public bool placeable = false;
    [Tooltip("Item that can be used in a special way. Ex: Key")]
    public bool useable = false;
    [Tooltip("OPTIONAL = the target layer to be detected and used by the item. Ex: log and the log drop zones.")]
    public LayerMask targetLayer;

    [Tooltip("Each item slot can stack X items. Set to 1 to make it not stackable")]
    public int maxStackSize = 1;

    public Sprite icon;

}
