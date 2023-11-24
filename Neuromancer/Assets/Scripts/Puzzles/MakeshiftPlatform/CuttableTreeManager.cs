using UnityEngine;

public class CuttableTreeManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private DestructibleObject tree;
    [SerializeField] private GameObject treeStump;
    [SerializeField] private GameObject itemDropControllerObject;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Transform targetDropFX;
    [SerializeField] private float itemDropYDirection = 3f;
    private string treeId;

    private void Awake()
    {
        if(tree != null)
        {
            tree.InitiateDestroy.AddListener(DestroyTree);
            treeId = tree.GetComponent<ObjectPermanent>().id;
        }
    }

    private void Update() {
        if (tree == null && treeStump.activeSelf == false) {
            treeStump.SetActive(true);
        }
    }

    // Requires reference to unit who hit to inform direction of where the log falls
    private void DestroyTree(GameObject unitAttacker)
    {
        Quaternion targetRotation = Quaternion.LookRotation(unitAttacker.transform.forward + (Vector3.up * itemDropYDirection));
        GameObject itemDrop = Instantiate(itemDropControllerObject, targetDropFX.position, targetRotation);
        ItemDropController itemDropController = itemDrop.GetComponent<ItemDropController>();
        itemDropController.item = itemData.itemPrefab;
        itemDropController.count = 1;
        itemDropController.uniqueId = treeId;
        ParticleSystem partSys = itemDrop.GetComponent<ParticleSystem>();
        var partSysSettings = partSys.main;
        partSysSettings.startSize = itemData.itemPrefab.transform.localScale.x;
    }
}
