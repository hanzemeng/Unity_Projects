using UnityEngine;

[CreateAssetMenu(menuName="SpawnerManagerSetting")]
public class SpawnerManagerSettings : ScriptableObject
{
    public float detectionRate = 1f;

    public float minDistance = 25f;
    public float spawnRegionWidth = 20f;
}
