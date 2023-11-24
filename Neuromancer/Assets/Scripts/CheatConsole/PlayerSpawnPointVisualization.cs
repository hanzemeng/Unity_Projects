using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerSpawnPoints))]
public class PlayerSpawnPointVisualization : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerSpawnerPrefab;
    private VisualizationStateManager instance;
    private PlayerSpawnPoints spawnPoints;

    private GameObject playerSpawnerVisualizer;
    private List<GameObject> allPlayerSpawnPointVisuals = new List<GameObject>();

    private void Awake()
    {
        instance = VisualizationStateManager.instance;
        playerSpawnerVisualizer = new("Player Spawn Point Visualizations");
        spawnPoints = GetComponent<PlayerSpawnPoints>();
    }
    
    private void Start()
    {
        // Instantiates a new prefab for each spawnPoint location:
        //playerSpawnerVisualizer.transform.position = gameObject.transform.localPosition;
        int counter = 0;
        foreach(GameObject spawnPoint in spawnPoints.playerSpawnPoints)
        {
            //Debug.Log("Entered the foreach loop");
            GameObject visual = Instantiate(playerSpawnerPrefab, spawnPoint.transform.position, Quaternion.identity);
            visual.transform.SetParent(playerSpawnerVisualizer.transform);
            visual.GetComponent<SpawnPointUIController>().SetIndexText(counter);
            visual.SetActive(instance.ShowPlayerSpawnPointVisuals);
            allPlayerSpawnPointVisuals.Add(visual);
            
            counter += 1;
        }

        instance.togglePlayerSpawnPointVisualizationsEvent.AddListener(TogglePlayerSpawnerVisual);

    }

    public void TogglePlayerSpawnerVisual(bool isEnabled)
    {
        // will enable and disable everything all at once.
        foreach(GameObject visual in allPlayerSpawnPointVisuals)
        {
            visual.SetActive(isEnabled);
        }
    }

    private void OnDestroy()
    {
        instance.togglePlayerSpawnPointVisualizationsEvent.RemoveListener(TogglePlayerSpawnerVisual);
    }
 
}
