using UnityEngine;

[CreateAssetMenu(menuName = "EnemyUnit")]
public class EnemyUnit : ScriptableObject
{
    public static int MAX_INVENTORY_SPACE = 1;

    public Neuromancer.NPCUnitType npcUnitType = Neuromancer.NPCUnitType.DEFAULT;

    public GameObject enemyPrefab;
    public GameObject allyPrefab;

    public bool canTakeOver = true;

    [Tooltip("EnemyUnit.MAX_INVENTORY_SPACE will cap the inventorySpace across all units. Edit the EnemyUnit.cs script to change the MAX_INVENTORY_SPACE")]
    public int inventorySpace = 0;

    public Sprite icon;

    public GameObject deathFX;
    public GameObject dropItemFX;
    public bool isBoss; 
}
