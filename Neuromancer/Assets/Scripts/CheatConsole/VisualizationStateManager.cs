using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Neuromancer;

// NOTE: This visualizer state manager may not be limited to navmesh visualizatio[CreateAssetMenu(fileName = "New NavMesh Visual Command", menuName = "DeveloperConsole/Commands/NavMesh Visual Command")]ns, can be extended to other visualiztions like player spawn points.
public class VisualizationStateManager : MonoBehaviour
{
    
    // Serializable data class to store the state of each agent type ID
    [System.Serializable]
    public class NavMeshState
    {
        public int agentTypeID;
        public bool isEnabled;

        public NavMeshState(int agentTypeID, bool isEnabled)
        {
            this.agentTypeID = agentTypeID; 
            this.isEnabled = isEnabled;
        }
    }

    // Stores the state of each agent type ID in a list to be referenced across scene transitions.
    [Header("NavMesh Visualization")]
    private bool createNavmeshVisualizations = false;        // Will default to true, allows Initialization to create navmesh visuals for every scene with a navmeshgenerator component
                                                            // Made in case the user wants to prevent performance dips when left checked off
    public bool CreateNavMeshVisualizations { get { return createNavmeshVisualizations; } }

    public List<NavMeshState> navMeshStates = new List<NavMeshState>();
    [SerializeField] private Material[] allNavMeshMaterials;

    // All preset navmeshAgentIDs:
    private List<int> navMeshAgentIDs = new List<int> 
    {
        0,
        -334000983,
        1479372276,
    };
      
    public static VisualizationStateManager instance;

    [System.Serializable] 
    public class ToggleNavMeshEvent : UnityEvent<int, bool> { }
    [System.Serializable]
    public class UpdateNavMeshEvent: UnityEvent<float> { }
    [System.Serializable]
    public class AllowNavMeshBuildEvent: UnityEvent<bool> { }

    [HideInInspector] public ToggleNavMeshEvent toggleNavMeshVisualizationsEvent = new ToggleNavMeshEvent();  // For turning on and off the navmesh visualizations
    [HideInInspector] public UpdateNavMeshEvent updateNavMeshVisualizationsEvent = new UpdateNavMeshEvent();  // For updating the navmesh visualizations, float variable for added delay
    [HideInInspector] public AllowNavMeshBuildEvent allowNavMeshBuildEvent = new AllowNavMeshBuildEvent();

    [Header("Breakable Visualization")]
    public Shader breakableShader;
    [SerializeField] [ColorUsage(true, true)]
    public Color neutralColor = Color.cyan;
    [SerializeField] [ColorUsage(true, true)]
    public Color bluntColor = Color.red;
    [SerializeField] [ColorUsage(true, true)]
    public Color sharpColor = Color.blue;

    private bool showBreakableVisuals = false;
    public bool ShowBreakableVisuals { get { return showBreakableVisuals;} } 
    [System.Serializable]
    public class ToggleBreakableEvent: UnityEvent<bool> { }
    [HideInInspector] public ToggleBreakableEvent toggleBreakableVisualizationsEvent = new ToggleBreakableEvent();

    
    private bool showPlayerSpawnPointVisuals = false;
    public bool ShowPlayerSpawnPointVisuals { get { return showPlayerSpawnPointVisuals; } }

    [Header("Player Spawn Point Visualizations")]
    [SerializeField] private GameObject playerSpawnPointPrefab;
    public class TogglePlayerSpawnerEvent: UnityEvent<bool> { }
    [HideInInspector] public TogglePlayerSpawnerEvent togglePlayerSpawnPointVisualizationsEvent = new TogglePlayerSpawnerEvent();

    private bool showUnitSpawnerVisuals = false;
    public bool ShowUnitSpawnerVisuals { get { return showUnitSpawnerVisuals; } }

    [Header("Unit Spawner Visualizations")]
    [SerializeField] private GameObject unitSpawnerVisualPrefab;
    public class ToggleUnitSpawnerEvent: UnityEvent<bool> {}
    [HideInInspector] public ToggleUnitSpawnerEvent toggleUnitSpawnerVisualizationEvent = new ToggleUnitSpawnerEvent();

    [Header("Unhide Chest Visualization")]
    [SerializeField] public Shader unhideShader;
    public Color unhideColor = Color.white;

    private bool unhideChestVisuals = false;
    public bool UnhideChestVisuals { get { return unhideChestVisuals;} }

    [System.Serializable]
    public class ToggleUnhideChestEvent: UnityEvent<bool> {}
    [HideInInspector] public ToggleUnhideChestEvent toggleUnhideChestVisualizationEvent = new ToggleUnhideChestEvent();

    // Singleton pattern
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;  

        toggleNavMeshVisualizationsEvent.AddListener(UpdateNavMeshState);
        allowNavMeshBuildEvent.AddListener(UpdateBuildNavMeshState);
        toggleBreakableVisualizationsEvent.AddListener(UpdateBreakable);
        togglePlayerSpawnPointVisualizationsEvent.AddListener(UpdatePlayerSpawnPoints);
        toggleUnitSpawnerVisualizationEvent.AddListener(UpdateUnitSpawn);
        toggleUnhideChestVisualizationEvent.AddListener(UpdateUnhideChestVisuals);
    
    }

    private void Start()
    {
        // Want to be flexible enough such that if the level manager is nonexistent, it likely means the it's scene specific.
        if(LevelManager.levelManager != null)
        {
            LevelManager.levelManager.onNewSceneEvent.AddListener(InitializeVisualizations);
            LevelManager.levelManager.onNewSceneLoadAsyncEvent.AddListener(CheckNavMeshInAsyncScene);
        }
        else
        {
            InitializeVisualizations();
        }

    }

    // Checks if the player has any ally units and if so, check if they need to destroy them:
    private void CheckNavMeshInAsyncScene(string sceneName)
    {
        // Will reference the SceneDebugManager to determine if a scene contains a navmesh
        if(!SceneDebugManager.instance.SceneContainsNavmesh(sceneName))
        {
            DestroyAlliesOfAgentID(removeAll: true);
        }
    }

    // Reponsible for initializing all the visualizations to be used by the debugger
    private void InitializeVisualizations()
    {
        Debug.Log("Initializing all visuals...");
        if(createNavmeshVisualizations)
        {
            InitializeNavMeshSurfaces();
        }
        
        InitializeBreakableVisuals();
        InitializePlayerSpawnPointVisuals();  
        InitializeUnitSpawnerVisuals();
        InitializeChestVisuals();
    }

    /* ---------------------------------------------------- */
    /* -------------- INITIALIZATION METHODS -------------- */
    /* ---------------------------------------------------- */

    // Initalize all navmesh surfaces to false when first loading a scene
    public void InitializeNavMeshSurfaces()
    {   
        NavMeshSurfaceGenerator navMeshGenerator = FindObjectOfType<NavMeshSurfaceGenerator>();
        //NavMeshData[] currentNavmesh = FindObjectsOfType<NavMeshData>();
        if(navMeshGenerator != null || navMeshGenerator.Surfaces.Length == 0)
        {
            bool doesValidNavmeshExist = true;

            // All 3 navmesh surfaces much exist in the current scene, else delete all ally units.
            foreach(Unity.AI.Navigation.NavMeshSurface surface in navMeshGenerator.Surfaces)
            {
                if(surface.navMeshData == null)
                {
                    doesValidNavmeshExist = false;
                    break;
                }
            }

            if(doesValidNavmeshExist)
            {
                // EnableAllyUnits(true);
                NavMeshVisualizer navMeshVisualizer = navMeshGenerator.gameObject.AddComponent<NavMeshVisualizer>();
                navMeshVisualizer.visualizationMaterials = allNavMeshMaterials;    
            }
            else
            {
                DeveloperConsoleController.AddStaticMessageToConsole($"No valid NavMesh surfaces found for the scene \"{SceneManager.GetActiveScene().name}\". All allies in the player's party will be destroyed.");
                if(UnitGroupManager.current.allUnits.units.Count > 0)
                {
                    DestroyAlliesOfAgentID(removeAll: true);
                }
            }
  
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"No NavMesh found for the scene \"{SceneManager.GetActiveScene().name}\". All allies in the player's party will be destroyed.");
            if(UnitGroupManager.current.allUnits.units.Count > 0)
            {
                DestroyAlliesOfAgentID(removeAll: true);
            }
        }

        if(navMeshStates.Count == 0)
        {
            foreach(int id in navMeshAgentIDs)
            {
                toggleNavMeshVisualizationsEvent?.Invoke(id, false);
            }
        }
        else
        {
            foreach(NavMeshState state in navMeshStates)
            {
                toggleNavMeshVisualizationsEvent?.Invoke(state.agentTypeID, state.isEnabled);
            }
        }

    }   

    private void InitializeBreakableVisuals()
    {
        // Find all objects with the Breakable type
        Breakable[] allBreakables = FindObjectsOfType<Breakable>();

        // Iterate through the array of breakables
        if(allBreakables != null && allBreakables.Length > 0)
        {
            foreach(Breakable obstacle in allBreakables)
            {
                ObstacleShaderController obstacleVisual = obstacle.gameObject.AddComponent<ObstacleShaderController>();
                obstacleVisual.breakableShader = breakableShader;
                obstacleVisual.neutralColor = neutralColor;
                obstacleVisual.bluntColor = bluntColor;
                obstacleVisual.sharpColor = sharpColor;
            } 
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"No breakable obstacles have been found in the scene: \"{SceneManager.GetActiveScene().name}\"");
        }

        toggleBreakableVisualizationsEvent?.Invoke(showBreakableVisuals);        
    }

    private void InitializePlayerSpawnPointVisuals()
    {
        // Find the current scene's player spawn points
        PlayerSpawnPoints currentPlayerSpawnPoints = FindObjectOfType<PlayerSpawnPoints>();

        if(currentPlayerSpawnPoints != null)
        {
            PlayerSpawnPointVisualization playerSpawnPointVisuals = currentPlayerSpawnPoints.gameObject.AddComponent<PlayerSpawnPointVisualization>();
            playerSpawnPointVisuals.playerSpawnerPrefab = playerSpawnPointPrefab;
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"No player spawn points have been found in the scene: \"{SceneManager.GetActiveScene().name}\"");
        }

        togglePlayerSpawnPointVisualizationsEvent?.Invoke(ShowPlayerSpawnPointVisuals); 
    }

    private void InitializeUnitSpawnerVisuals()
    {
        // Find the current scene's ImprovedUnitSpawnerManager
        ImprovedUnitSpawnerManager spawnManager = FindObjectOfType<ImprovedUnitSpawnerManager>();

        if(spawnManager != null)
        {
            UnitSpawnerVisualization unitSpawnVisuals = spawnManager.gameObject.AddComponent<UnitSpawnerVisualization>();
            unitSpawnVisuals.spawnerPrefab = unitSpawnerVisualPrefab;
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"No unit spawners have been found in the scene: \"{SceneManager.GetActiveScene().name}\"");
        }
       
        toggleUnitSpawnerVisualizationEvent?.Invoke(showUnitSpawnerVisuals);
    }

    private void InitializeChestVisuals()
    {
        ChestController[] allChests = FindObjectsOfType<ChestController>();

        // Iterate through the array of breakables
        if(allChests != null && allChests.Length > 0)
        {
            foreach(ChestController chest in allChests)
            {
                TreasureChestRevealController chestVisual = chest.gameObject.AddComponent<TreasureChestRevealController>();
                chestVisual.unhideShader = unhideShader;
                chestVisual.unhideColor = unhideColor;

            } 
        }

        toggleUnhideChestVisualizationEvent?.Invoke(unhideChestVisuals);      
    }

    /* ---------------------------------------------------- */
    /* ---------------------------------------------------- */
    /* ---------------------------------------------------- */


    /* ---------------------------------------------------- */
    /* ---------- UPDATE VISUALIZATIONS METHODS ----------- */
    /* ---------------------------------------------------- */
    public void UpdateNavMeshState(int agentTypeID, bool isEnabled)
    {
        // Checks if the agentTypeID exists in the list and updates its state accordingly.
        int index = navMeshStates.FindIndex(state => state.agentTypeID == agentTypeID);
        if(index >= 0)
        {
            navMeshStates[index].isEnabled = isEnabled;
        }
        else 
        {
            navMeshStates.Add(new NavMeshState(agentTypeID, isEnabled));
        }
    }    

    public void UpdateBreakable(bool isEnabled)
    {
        showBreakableVisuals = isEnabled;
    }

    public void UpdatePlayerSpawnPoints(bool isEnabled)
    {
        showPlayerSpawnPointVisuals = isEnabled;
    }

    public void UpdateUnitSpawn(bool isEnabled)
    {
        showUnitSpawnerVisuals = isEnabled;
    }

    public void UpdateUnhideChestVisuals(bool isEnabled)
    {
        unhideChestVisuals = isEnabled;
    }

    // Whenever the update BuildNavMeshState toggled is disabled, will force each scene on reload to rebuild the scene's navmesh for the visualization.
    public void UpdateBuildNavMeshState(bool isEnabled)
    {
        createNavmeshVisualizations = isEnabled;
        if(createNavmeshVisualizations)
        {
            NavMeshVisualizer currentVisualizer = FindObjectOfType<NavMeshVisualizer>();
            // if there does not exist a navmesh visualizer component in the current scene.
            if(currentVisualizer == null)
            {
                InitializeNavMeshSurfaces();
            }
        }
    }

    /* ---------------------------------------------------- */
    /* ---------------------------------------------------- */
    /* ---------------------------------------------------- */

    public void DestroyAlliesOfAgentID(int agentID = 0, bool removeAll = false)
    {
        if(UnitGroupManager.current.allUnits.units.Count > 0)
        {
            // Removes all units from the player's party, then destroys them.
            NPCUnit[] allAllies = UnitGroupManager.current.allUnits.units.ToArray();
            foreach(Neuromancer.NPCUnit unit in allAllies)
            {
                // Checks for two things:
                // 1.) if the user want to remove all units.
                // 2.) if the user's agentID matches the inputted ID.
                if(!removeAll && unit.GetComponent<NavMeshAgent>().agentTypeID != agentID)
                {
                    continue;
                }

                // unit.transform.root.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(killDamage, EmeraldAISystem.TargetType.AI, unit.transform, 100);
                foreach (UnitGroup uG in unit.unitGroups)
                {
                    Debug.Log("Removing ally from Unit Group");
                    uG.RemoveUnit(unit);
                }
                // Remove from party then destroy:
                UnitGroupManager.current.RemoveUnit(unit);
                // Destroy(unit.gameObject); 
            }
        }
    }

    private void OnDestroy() 
    {
        toggleNavMeshVisualizationsEvent.RemoveListener(UpdateNavMeshState);
        allowNavMeshBuildEvent.RemoveListener(UpdateBuildNavMeshState);
        toggleBreakableVisualizationsEvent.RemoveListener(UpdateBreakable);
        togglePlayerSpawnPointVisualizationsEvent.RemoveListener(UpdatePlayerSpawnPoints);
        toggleUnitSpawnerVisualizationEvent.RemoveListener(UpdateUnitSpawn);
        toggleUnhideChestVisualizationEvent.RemoveListener(UpdateUnhideChestVisuals);

        if(LevelManager.levelManager != null)
        {
            LevelManager.levelManager.onNewSceneEvent.RemoveListener(InitializeVisualizations);
            LevelManager.levelManager.onNewSceneLoadAsyncEvent.RemoveListener(CheckNavMeshInAsyncScene);
        }
        
    }

    private void EnableAllyUnits(bool isEnabled)
    {
        // Find all units and disables their EmeraldAI component
        
        if(UnitGroupManager.current.allUnits.units.Count > 0)
        {
            // Disables all allies's NavMeshAgent component.
            foreach(Neuromancer.NPCUnit unit in UnitGroupManager.current.allUnits.units)
            {
                unit.GetComponent<NavMeshAgent>().enabled = isEnabled;
                
                unit.EmeraldComponent.enabled = isEnabled;                
            }
        }
        
    }

}
