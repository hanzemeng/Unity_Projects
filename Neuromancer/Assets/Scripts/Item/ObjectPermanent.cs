using UnityEngine;
using CleverCrow.Fluid.UniqueIds;

[RequireComponent(typeof(UniqueId))]
public class ObjectPermanent : MonoBehaviour
{
    [HideInInspector] public string id;

    private void Awake() {
        id = GetComponent<UniqueId>().Id;
    }

    private void OnEnable() {
        ObjectData myData = new ObjectData(id, 0);
        SaveLoadManager.saveLoadManager.AddObjectEntry(myData);
    }

    public void SetDisable() {
        ObjectData myData = new ObjectData(id, 1);
        SaveLoadManager.saveLoadManager.ModObjectEntry(myData);
    }

    public void SetDestroy() {
        ObjectData myData = new ObjectData(id, 2);
        SaveLoadManager.saveLoadManager.ModObjectEntry(myData);
    }

    public static void SetDestroyID(string idToDestroy) {
        ObjectData myData = new ObjectData(idToDestroy, 2);
        SaveLoadManager.saveLoadManager.ModObjectEntry(myData);
    }
}

public class ObjectData
{
    public string id;
    public int state; // 0 for enabled, 1 for disabled, 2 for destroyed
    public ObjectData(string i, int s) {
        id = i;
        state = s;
    }
    public string ToJsonString() { return JsonUtility.ToJson(this); }
}
