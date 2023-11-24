using System.Collections.Generic;
using UnityEngine;

public class ImprovedUnitSpawnerManager : MonoBehaviour
{
    public static ImprovedUnitSpawnerManager current;

    public SpawnerManagerSettings settings;

    private float nextDetection;
    private List<ImprovedUnitSpawner> spawners = new List<ImprovedUnitSpawner>();
    public List<ImprovedUnitSpawner> Spawners { get { return spawners; }}

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this);
        }
        else
        {
            current = this;
        }
    }

    private void FixedUpdate()
    {
        if (Time.time > nextDetection)
        {
            nextDetection = Time.time + settings.detectionRate;
            foreach (ImprovedUnitSpawner ius in spawners)
            {
                float dist = Vector3.Distance(PlayerController.player.transform.position, ius.transform.position);
                if (dist > settings.minDistance && dist < settings.minDistance + settings.spawnRegionWidth)
                {
                    ius.isActive = true;
                }
                else
                {
                    ius.isActive = false;
                }
            }
        }
    }

    public void AddSpawner(ImprovedUnitSpawner ius)
    {
        spawners.Add(ius);
    }
}
