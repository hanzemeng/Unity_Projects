using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ImprovedUnitSpawnerManager))]
public class UnitSpawnerVisualization : MonoBehaviour
{
    public GameObject spawnerPrefab;

    private VisualizationStateManager instance;
    private ImprovedUnitSpawnerManager currentUnitSpawner;
    private GameObject spawnerVisualization;
    private GameObject spawnerVarianceVisual;
    private GameObject spawnerMinDistanceVisual;
    private GameObject spawnerMinDistance_plus_RegionWidth;

    private List<GameObject> allSpawnerVisualizers = new List<GameObject>();

    private void Awake()
    {

        instance = VisualizationStateManager.instance;
        spawnerVisualization = new("Unit Spawner Visualizations");
        currentUnitSpawner = GetComponent<ImprovedUnitSpawnerManager>();
    }
    // Start is called before the first frame update
    private void Start()
    {   
        // Instantiates a new prefab for each spawnPoint location:
        int counter = 0;
        foreach(ImprovedUnitSpawner spawnPoint in currentUnitSpawner.Spawners)
        {
            //Debug.Log("Entered the foreach loop");
            GameObject visual = Instantiate(spawnerPrefab, spawnPoint.transform.position, Quaternion.identity);
            visual.transform.SetParent(spawnerVisualization.transform);
            SpawnPointUIController visualUI = visual.GetComponent<SpawnPointUIController>();
            visualUI.SetIndexText(counter);
            visualUI.InitializeUnitSpawnerVisuals
            (
                spawnPoint.spawnPointVariance, 
                currentUnitSpawner.settings.minDistance, 
                currentUnitSpawner.settings.minDistance + currentUnitSpawner.settings.spawnRegionWidth, 
                spawnPoint
            );
            visual.SetActive(instance.ShowUnitSpawnerVisuals);
            allSpawnerVisualizers.Add(visual);
            
            counter += 1;
        }

        instance.toggleUnitSpawnerVisualizationEvent.AddListener(ToggleUnitSpawnerVisual);
    }

    public void ToggleUnitSpawnerVisual(bool isEnabled)
    {
        // will enable and disable everything all at once.
        foreach(GameObject visual in allSpawnerVisualizers)
        {
            visual.SetActive(isEnabled);
        }
    }

    private void OnDestroy()
    {
        instance.togglePlayerSpawnPointVisualizationsEvent.RemoveListener(ToggleUnitSpawnerVisual);
    }


}
