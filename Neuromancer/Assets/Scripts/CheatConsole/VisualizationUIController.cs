using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VisualizationUIController : MonoBehaviour
{
    private VisualizationStateManager stateManager;
    
    // References to the UI Toggles for each of the navmesh surfaces
    [Header("All toggles for NavMesh display")]
    [SerializeField] private Toggle buildNavmeshToggle;
    [SerializeField] private Toggle humanoidToggle;
    [SerializeField] private Toggle flyerToggle;
    [SerializeField] private Toggle swimmerToggle;
    [SerializeField] private Toggle allToggle;
    [SerializeField] private Button updateNavmeshButton;

    [Tooltip("The delay time on updating the NavMesh visuals after the button has been pressed.")]
    [SerializeField] private float navMeshUpdateDelayTime = 0.5f;
    private Dictionary<Toggle, int> allIndividualToggles = new Dictionary<Toggle, int>();

    [Header("Other Visual Toggles")]
    [SerializeField] private Toggle breakableToggle;
    [SerializeField] private Toggle playerSpawnPointsToggle;
    [SerializeField] private Toggle unitSpawnerToggle;
    [SerializeField] private Toggle unhideChestToggle;
    private bool processingAllToggle = false;

    private void Awake()
    {
        // acquires a dictionary of all toggles for the allToggle
        allIndividualToggles = new Dictionary<Toggle, int>
        {
            {humanoidToggle, 0},
            {flyerToggle, -334000983},
            {swimmerToggle, 1479372276}
        };

        // NavMesh Visualization Toggles
        foreach (var kvp in allIndividualToggles)
        {
            kvp.Key.onValueChanged.AddListener(OnIndividualToggleValueChanged);
        }
        allToggle.onValueChanged.AddListener(OnAllToggleValueChanged);

        buildNavmeshToggle.onValueChanged.AddListener(OnBuildNavMeshToggleValueChanged);

        // Breakable Visualization Toggle 
        breakableToggle.onValueChanged.AddListener(OnBreakableToggleValueChanged);

        // Player Spawn Points Visualization Toggle
        playerSpawnPointsToggle.onValueChanged.AddListener(OnPlayerSpawnPointToggleValueChanged);

        // Unit Spawner Visualization Toggle
        unitSpawnerToggle.onValueChanged.AddListener(OnUnitSpawnerToggleValueChanged);

        // Unhide Chest Visualization Toggle
        unhideChestToggle.onValueChanged.AddListener(OnUnhideChestToggleValueChanged);

    }

    private void Start()
    {
        // Subscribes to the navmesh surface state change on the State Manager
        stateManager = VisualizationStateManager.instance;
        
        // Navmesh related toggles:
        stateManager.toggleNavMeshVisualizationsEvent.AddListener(OnNavMeshStateChanged);
        OnBuildNavMeshStateChanged(stateManager.CreateNavMeshVisualizations);
        stateManager.allowNavMeshBuildEvent.AddListener(OnBuildNavMeshStateChanged);
        if(!stateManager.CreateNavMeshVisualizations)
        {
            OnBuildNavMeshToggleValueChanged(stateManager.CreateNavMeshVisualizations);
        }
        
        
        // Other toggles:
        stateManager.toggleBreakableVisualizationsEvent.AddListener(OnBreakableStateChanged);
        stateManager.togglePlayerSpawnPointVisualizationsEvent.AddListener(OnPlayerSpawnPointStateChanged);
        stateManager.toggleUnitSpawnerVisualizationEvent.AddListener(OnUnitSpawnerStateChanged);
        stateManager.toggleUnhideChestVisualizationEvent.AddListener(OnUnhideChestStateChanged);
    }

    // --------- NAVMESH TOGGLES --------- //
    private void OnIndividualToggleValueChanged(bool isOn)
    {
        if(processingAllToggle)
        {
            // skips updating individual toggles if it is currently being used.
            return;
        }
        // Acquire the current toggle that has triggered the event itself
        Toggle toggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();

        // Get the associated agentTypeID from the dictionary, null check is very important in case the toggle isn't what triggered the event
        if(toggle != null)
        {
            int agentTypeID = allIndividualToggles[toggle];
            stateManager.toggleNavMeshVisualizationsEvent?.Invoke(agentTypeID, isOn);
        }

    }

    private void OnAllToggleValueChanged(bool isOn)
    {   
        processingAllToggle = true;
        foreach(var kvp in allIndividualToggles)
        {
            kvp.Key.onValueChanged.RemoveListener(OnIndividualToggleValueChanged);
            stateManager.toggleNavMeshVisualizationsEvent?.Invoke(kvp.Value, isOn);
            //kvp.Key.isOn = isOn;
            kvp.Key.onValueChanged.AddListener(OnIndividualToggleValueChanged);
        }
        processingAllToggle = false;
    }   

    
    private void OnBuildNavMeshToggleValueChanged(bool isOn)
    {
        if(isOn == false)
        {
            OnAllToggleValueChanged(false);
            stateManager.allowNavMeshBuildEvent?.Invoke(false);
        }
        else
        {
            stateManager.allowNavMeshBuildEvent?.Invoke(true);
        }

        // Enable/Disable all related navmesh toggles and buttons.
        foreach(var kvp in allIndividualToggles)
        {
            kvp.Key.interactable = isOn;
        }
        allToggle.interactable = isOn;
        updateNavmeshButton.interactable = isOn;
    }

    private void OnBuildNavMeshStateChanged(bool isEnabled)
    {
        buildNavmeshToggle.isOn = isEnabled;
    }

    // ------------------------------------ //

    // ----------- OTHER TOGGLES ---------- //

    private void OnBreakableToggleValueChanged(bool isOn)
    {
        stateManager.toggleBreakableVisualizationsEvent?.Invoke(isOn);
    }

    private void OnBreakableStateChanged(bool isEnabled)
    {
        breakableToggle.isOn = isEnabled;
    }

    private void OnPlayerSpawnPointToggleValueChanged(bool isOn)
    {
        stateManager.togglePlayerSpawnPointVisualizationsEvent?.Invoke(isOn);
    }

    private void OnPlayerSpawnPointStateChanged(bool isEnabled)
    {
        playerSpawnPointsToggle.isOn = isEnabled;
    }

    private void OnUnitSpawnerToggleValueChanged(bool isOn)
    {
        stateManager.toggleUnitSpawnerVisualizationEvent?.Invoke(isOn);
    }

    private void OnUnitSpawnerStateChanged(bool isEnabled)
    {
        unitSpawnerToggle.isOn = isEnabled;
    }

    private void OnUnhideChestToggleValueChanged(bool isOn)
    {
        stateManager.toggleUnhideChestVisualizationEvent?.Invoke(isOn);
    }

    private void OnUnhideChestStateChanged(bool isEnabled)
    {
        unhideChestToggle.isOn = isEnabled;
    }
    
    // Should automatically update the checkmarks to reflect any changes to the navmesh displays via console commands
    public void OnNavMeshStateChanged(int agentTypeID, bool isEnabled)
    {
        switch(agentTypeID)
        {
            case 0:
                humanoidToggle.isOn = isEnabled;
                break;
            case -334000983:
                flyerToggle.isOn = isEnabled;
                break;
            case 1479372276:
                swimmerToggle.isOn = isEnabled;
                break;
        }
        UpdateAllToggle();

    }

    // Used to update the All toggle accordingly such that if at least one of the toggles is not on, have the AllToggle be set off as well.
    private void UpdateAllToggle()
    {
        // have the allToggle unsubscribe from the event so that it doesnt overlap with other events
        allToggle.onValueChanged.RemoveListener(OnAllToggleValueChanged);
        foreach(var kvp in allIndividualToggles)
        {
            if(!kvp.Key.isOn)
            {   
                allToggle.isOn = false;
                allToggle.onValueChanged.AddListener(OnAllToggleValueChanged);
                return;
            }
        }
        allToggle.isOn = true;
        allToggle.onValueChanged.AddListener(OnAllToggleValueChanged);
    }

    // Used by the "Update Display" button in the DeveloperConsole UI, simply triggers event for navMeshVisualizer gameObject in each scene.
    public void UpdateNavMeshVisualization()
    {
        stateManager.updateNavMeshVisualizationsEvent?.Invoke(navMeshUpdateDelayTime);
    }

    // ---------------------------------- //
    private void OnDestroy()
    {
        // -- TOGGLE EVENTS -- //
        // NavMesh Visualization Toggles
        foreach (var kvp in allIndividualToggles)
        {
            kvp.Key.onValueChanged.RemoveListener(OnIndividualToggleValueChanged);
        }
        allToggle.onValueChanged.RemoveListener(OnAllToggleValueChanged);

        buildNavmeshToggle.onValueChanged.RemoveListener(OnBuildNavMeshToggleValueChanged);

        // Breakable Visualization Toggle 
        breakableToggle.onValueChanged.RemoveListener(OnBreakableToggleValueChanged);

        // Player Spawn Points Visualization Toggle
        playerSpawnPointsToggle.onValueChanged.RemoveListener(OnPlayerSpawnPointToggleValueChanged);

        // Unit Spawner Visualization Toggle
        unitSpawnerToggle.onValueChanged.RemoveListener(OnUnitSpawnerToggleValueChanged);

        // Unhide Chest Visualization Toggle
        unhideChestToggle.onValueChanged.RemoveListener(OnUnhideChestToggleValueChanged);

        
        // -- UPDATE STATE EVENTS -- //
        stateManager.toggleNavMeshVisualizationsEvent.RemoveListener(OnNavMeshStateChanged);
        stateManager.allowNavMeshBuildEvent.RemoveListener(OnBuildNavMeshStateChanged);
        
        // Other toggles:
        stateManager.toggleBreakableVisualizationsEvent.RemoveListener(OnBreakableStateChanged);
        stateManager.togglePlayerSpawnPointVisualizationsEvent.RemoveListener(OnPlayerSpawnPointStateChanged);
        stateManager.toggleUnitSpawnerVisualizationEvent.RemoveListener(OnUnitSpawnerStateChanged);
        stateManager.toggleUnhideChestVisualizationEvent.RemoveListener(OnUnhideChestStateChanged);
    }
    
}
