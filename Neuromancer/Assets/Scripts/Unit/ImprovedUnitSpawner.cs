using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedUnitSpawner : MonoBehaviour
{
    public bool alwaysActive;
    public bool isActive = false;

    public float minSpawnRate = 5f;
    public float maxSpawnRate = 10f;

    private float nextSpawnTime;

    [Tooltip("Keep this value small on uneven ground")]
    public float spawnPointVariance = 1f;

    [Tooltip("Max number of concurrent units spawned from this spawner.")]
    public int maxUnitsAlive = 1;
    private int currentUnitsAlive = 0;

    public Transform rallyPoint;

    public List<GameObject> unitPrefabs = new();
    private List<GameObject> spawnedUnits = new();

    private System.Random randomIndexGenerator;

    private List<Transform> spawnLocations = new List<Transform>();

    public SpawnerManagerSettings failsafeSettings;

    [Header("Spawning Effects")]
    [SerializeField] private GameObject pulse;
    [SerializeField] private GameObject smoke;
    [SerializeField] private float waitTime = 3f;
    private GameObject smokeAudio;
    private GameObject growlAudio;

    private void Start()
    {
        // Fail safe
        if (ImprovedUnitSpawnerManager.current == null)
        {
            ImprovedUnitSpawnerManager manager = gameObject.AddComponent<ImprovedUnitSpawnerManager>();
            ImprovedUnitSpawnerManager.current = manager;
            if (failsafeSettings != null)
                manager.settings = failsafeSettings;
            else
                manager.settings = new SpawnerManagerSettings();
        }

        if (unitPrefabs.Count <= 0)
        {
            Debug.LogError("Deactivated Spawner... You must have at least one unit in 'unitPrefabs' list");
            gameObject.SetActive(false);
        }

        spawnedUnits = new List<GameObject>();

        ImprovedUnitSpawnerManager.current.AddSpawner(this);
        randomIndexGenerator = new System.Random();

        Transform[] children = GetComponentsInChildren<Transform>();
        spawnLocations = new List<Transform>();
        if (children.Length <= 1)
            spawnLocations.Add(transform);
        else
        {
            for (int i = 1; i < children.Length; i++)
            {
                spawnLocations.Add(children[i]);
            }
        }

        smokeAudio = AudioManager.instance.MusicAttached(AudioManager.SoundResource.SPAWN_SMOKE, transform);
        growlAudio = AudioManager.instance.MusicAttached(AudioManager.SoundResource.MONSTER_GROWL, transform);
    }

    private void FixedUpdate()
    {
        isActive |= alwaysActive;
        if (!isActive)
        {
            return;
        }

        if (Time.time < nextSpawnTime)
        {
            return;
        }

        // Time to spawn!
        if (currentUnitsAlive < maxUnitsAlive) // Check if spawning conditions are met
        {
            StartCoroutine(SpawnUnit());
        }
    }

    public void RemoveUnit(GameObject unit)
    {
        if (spawnedUnits.Contains(unit))
        {
            spawnedUnits.Remove(unit);
            currentUnitsAlive--;
            nextSpawnTime = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform c in spawnLocations)
        {
            Gizmos.DrawWireSphere(c.position, spawnPointVariance);
        }
    }

    private IEnumerator SpawnUnit() {
        GameObject unit = unitPrefabs[randomIndexGenerator.Next(0, unitPrefabs.Count)];

        nextSpawnTime = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
        Transform randomSpawnLocation = spawnLocations[randomIndexGenerator.Next(0, spawnLocations.Count)];
        Vector3 variance = new Vector3(Random.onUnitSphere.x, 0f, Random.onUnitSphere.z) * spawnPointVariance;
        Vector3 spawnLocation = randomSpawnLocation.position + variance;

        // Spawning effects
        Vector3 groundOffset = new(0f, 0.1f, 0f);
        Instantiate(pulse, spawnLocation + groundOffset, pulse.transform.rotation);
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayMusic(growlAudio);
        yield return new WaitForSeconds(waitTime - 1f);
        Instantiate(smoke, spawnLocation + groundOffset, smoke.transform.rotation);
        AudioManager.instance.PlayMusic(smokeAudio);
        yield return new WaitForSeconds(0.1f);

        GameObject newUnit = Instantiate(unit, spawnLocation, Quaternion.identity);
        spawnedUnits.Add(newUnit);
        currentUnitsAlive++;

        newUnit.GetComponent<Neuromancer.NPCUnit>().sourceSpawner = this;

        if(null != rallyPoint)
        {
            newUnit.GetComponent<EmeraldAI.EmeraldAISystem>().EmeraldEventsManagerComponent.SetDestination(rallyPoint);
        }
    }

}
