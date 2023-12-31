using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Unity.Jobs;   // Necessary in order to reduce performance dip when rebaking scene

// Generates a mesh for visualizations of each navmesh surface with the NavMeshSurfaceGenerator
// Must be attached to the NavMeshGenerator gameObject, visualizes NavMesh per scene based on DeveloperConsole commands (invoke event)
[RequireComponent(typeof(NavMeshSurfaceGenerator))]
public class NavMeshVisualizer : MonoBehaviour
{
    [System.Serializable]
    public struct NavMeshVisualizerData
    {
        public int agentTypeID;
        public GameObject visualContainer;
        public bool isEnabled;

        public NavMeshVisualizerData(int id, GameObject visual, bool enableFlag)
        {
            this.agentTypeID = id;
            this.visualContainer = visual;
            this.isEnabled = enableFlag;
        }

        public void ActivateGameObject(bool enableFlag)
        {
            isEnabled = enableFlag;
            visualContainer.SetActive(enableFlag);
        }
    }

    [Header("All Navmesh Visualization Booleans")]
    public bool showAllVisualizations = true;

    [Header("On/Off Switch for Navmesh Visualization")]
    public bool isVisualizationToggled = true;

    [Header("Navmesh Mesh Generation Settings")]
    public Material[] visualizationMaterials;
    [SerializeField] private Vector3 generatedMeshOffset = new Vector3(0, 0.18f, 0);

    [Header("Timer before Updating Navmesh")]
    [SerializeField] private float updateWaitTime = 1.2f;
    private bool isProcessingUpdate = false;
    private IEnumerator updateCoroutine;

    [Header("All Events Potentially Able to Update Navmesh")]
    private NavMeshSurface[] surfaces;

    // will be used to track any changes made to the navmeshes and update accordingly
    private Dictionary<NavMeshSurface, NavMeshData> navMeshDataDictionary = new Dictionary<NavMeshSurface, NavMeshData>();
    private Dictionary<int, NavMeshVisualizerData> navMeshVisualContainers = new Dictionary<int, NavMeshVisualizerData>(); 
    private NavMeshVisualizerData[] visualizerDatas;

    // A gameObject created during Start(), will be inserted into the scene hierarchy.
    private GameObject meshVisualization;

    // Grabs an instance of the VisualizationStateManager if it exists:
    private VisualizationStateManager stateManager;

    private void Awake()
    {
        meshVisualization = new("NavMesh Visualization");
        meshVisualization.transform.position = generatedMeshOffset;
        surfaces = NavMeshSurfaceGenerator.current.Surfaces;    // acquires all the Navmeshes generated by the NavMeshSurfaceGenerator instance in current scene
        stateManager = VisualizationStateManager.instance;
    }

    private void OnEnable()
    {
        if(stateManager != null)
        {
            stateManager.toggleNavMeshVisualizationsEvent.AddListener(ToggleNavMeshVisualization);
            stateManager.updateNavMeshVisualizationsEvent.AddListener(UpdateNavMeshVisualization);
        }
    }

    private void OnDisable()
    {
        if(stateManager != null)
        {
            stateManager.toggleNavMeshVisualizationsEvent.RemoveListener(ToggleNavMeshVisualization);
            stateManager.updateNavMeshVisualizationsEvent.RemoveListener(UpdateNavMeshVisualization);
        }
    }
    
    private void Start()
    {
        UpdateNavMeshVisualizer();
    }

    private void ToggleNavMeshVisualization(int id, bool isEnabled)
    {
        // Finds the child gameObject representing the navmesh visualization
        Transform agentVisualization = meshVisualization.transform.Find($"Visualization for Agent {id}");

        if(agentVisualization != null)
        {
            agentVisualization.gameObject.SetActive(isEnabled);
        }
    }

    private void UpdateNavMeshVisualization(float delay)
    {
        DeveloperConsoleController.AddStaticMessageToConsole("Rebuilding visualization of scene's navmesh, please wait...");
        StartCoroutine(NavMeshUpdateCoroutine(delay));
    }

    // Will be used to update the navmesh visualizer every time a gameObject changes
    private void UpdateNavMeshVisualizer()
    {
        EnableAllUnits(false);

        bool[] isValidNavmeshSurface = new bool[surfaces.Length];
        int currentIndex = 0;

        foreach(NavMeshSurface surface in surfaces)
        {   

            // Look for whether or not a visualization for the gameObject exists for the surface, if not
            Transform visualCheck = meshVisualization.transform.Find($"Visualization for Agent {surface.agentTypeID}");
            GameObject visualization;
            if(visualCheck != null)
            {
                visualization = visualCheck.gameObject;
            }
            else
            {
                visualization = new($"Visualization for Agent {surface.agentTypeID}");
            }
            visualization.transform.SetParent(meshVisualization.transform, false);

            if(surface.navMeshData == null)
            {
                DeveloperConsoleController.AddStaticMessageToConsole($"There is no valid navmesh surface available for the NavMesh Agent {surface.agentTypeID}, \nNo visualization for this agent will be generated.");
                isValidNavmeshSurface[currentIndex] = false;
                currentIndex += 1;
                stateManager.DestroyAlliesOfAgentID(surface.agentTypeID, false);
                continue;
            }
            isValidNavmeshSurface[currentIndex] = true;
            currentIndex += 1;

            // For each gameObject represented for each visualization, add a MeshRenderer and MeshFilter component if it already doesnt exist
            MeshRenderer renderer = visualization.GetComponent<MeshRenderer>();
            MeshFilter filter = visualization.GetComponent<MeshFilter>();
            if(renderer == null && filter == null)
            {
                renderer = visualization.AddComponent<MeshRenderer>();
                filter = visualization.AddComponent<MeshFilter>();
            }
            
            // NOTE: this must be done because Unity's Triangulation method is stupid and can't distiguish vertices associated for specific agent types
            NavMesh.RemoveAllNavMeshData();
            surface.BuildNavMesh();

            DrawNavMeshVisualizer(visualization);
            // visualization.transform.SetParent(meshVisualization.transform, false);

            // Ensures that the previous settings made in prior scenes carry over into the current scene when loaded.
            if(stateManager != null)
            {
                int agentID = surface.agentTypeID;
                bool isEnabled = stateManager.navMeshStates.Find(state => state.agentTypeID == agentID)?.isEnabled ?? false;
                visualization.SetActive(isEnabled);
            }
        }

        // Rebuild navmesh data all over again :(
        currentIndex = 0;
        NavMesh.RemoveAllNavMeshData();
        foreach(NavMeshSurface surface in surfaces)
        {
            if(isValidNavmeshSurface[currentIndex] == false)
            {
                continue;
            }

            surface.BuildNavMesh();

            // Store the current navmesh references in the dictionary to be checked and updated later if changes are made in scene.
            NavMeshData surfaceData = surface.navMeshData;
            if(!navMeshDataDictionary.ContainsKey(surface))
            {
                navMeshDataDictionary.Add(surface, surfaceData);
            }
        }

        EnableAllUnits(true);
    }

    // Updates the meshes used to represent the navmesh visualizers
    private void DrawNavMeshVisualizer(GameObject visualization)
    {
        // Acquires all the triangles and its respective indices to compute the mesh to be displayed:
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // For each gameObject represented for each visualization, add a MeshRenderer and MeshFilter component 
        MeshRenderer renderer = visualization.GetComponent<MeshRenderer>();
        MeshFilter filter = visualization.GetComponent<MeshFilter>();

        // Define a dictionary to contain all the indices per area (humanoid, swimmer, and flier)
        Dictionary<int, List<int>> areaIndices = new();
        for(int i = 0; i < triangulation.areas.Length; i++)
        {
            if(!areaIndices.ContainsKey(triangulation.areas[i]))
            {
                areaIndices.Add(triangulation.areas[i], new());
            }

            // Triangles = 3 indices, so add the three indices
            areaIndices[triangulation.areas[i]].Add(triangulation.indices[3 * i]);
            areaIndices[triangulation.areas[i]].Add(triangulation.indices[3 * i + 1]);
            areaIndices[triangulation.areas[i]].Add(triangulation.indices[3 * i + 2]);
        }

        // Mesh to represent the navmesh in real time:
        Mesh navMeshVisual = new Mesh();

        navMeshVisual.subMeshCount = areaIndices.Count;
        Material[] materials = new Material[areaIndices.Count];
        navMeshVisual.SetVertices(triangulation.vertices);

        int index = 0;
        foreach(KeyValuePair<int, List<int>> keyValuePair in areaIndices)
        {
            // Defines the triangles for the mesh.
            navMeshVisual.SetTriangles(keyValuePair.Value, index);  
            Material material = visualizationMaterials[0];
            if(keyValuePair.Key > 2)
            {
                // Any areas like Climable, Flyable and Swimmable have an area index of 3 and above.
                material = visualizationMaterials[keyValuePair.Key - 2];
            }

            materials[index] = material;
            index++;
        }
        renderer.sharedMaterials = materials;
        filter.mesh = navMeshVisual;            
    }

    // Used to toggle all or none of the units based on the inputted boolean
    private void EnableAllUnits(bool isEnabled)
    {
        // Find all units and disables their EmeraldAI component
        Neuromancer.NPCUnit[] allUnits = FindObjectsByType<Neuromancer.NPCUnit>(FindObjectsSortMode.None);
        if(allUnits.Length > 0)
        {
            // Just so we dont see a lot of warning pop ups occuring.
            foreach (Neuromancer.NPCUnit unit in allUnits) 
            {
                unit.GetComponent<NavMeshAgent>().enabled = isEnabled;
                unit.EmeraldComponent.enabled = isEnabled;
            }
        }
    }

    private IEnumerator NavMeshUpdateCoroutine(float waitTime)
    {
        if(!isProcessingUpdate)
        {
            isProcessingUpdate = true;
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(waitTime);
            UpdateNavMeshVisualizer();
            DeveloperConsoleController.AddStaticMessageToConsole("Visualization update complete! Please wait a bit for the cooldown.");
            yield return new WaitForSeconds(updateWaitTime);
            DeveloperConsoleController.AddStaticMessageToConsole("Visualization cooldown complete!");
            isProcessingUpdate = false;
        }
        
    }

    private void OnDestroy() 
    {
        if(stateManager != null)
        {
            stateManager.toggleNavMeshVisualizationsEvent.RemoveListener(ToggleNavMeshVisualization);
            stateManager.updateNavMeshVisualizationsEvent.RemoveListener(UpdateNavMeshVisualization);
        }
    }
}


