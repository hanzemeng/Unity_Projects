using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Neuromancer;
using UnityEngine.AI;
using System.Linq;

// Handles the following for the DeveloperManager:
//  1.) Stores all the names for each scene in build, their index, and stores how many player spawn points are available
//  2.) Acquires the current scene's name, also prevent access to very specific scenes (Initialization, Title, etc.)
//  3.) Determine if the scene contains a NavMesh, player Spawn Points, and Unit Spawners.
//  4.) Will be accessed by the ToSceneCommand
[RequireComponent(typeof(VisualizationStateManager))]
public class SceneDebugManager : MonoBehaviour
{
    [System.Serializable]
    public class LoadLevelData
    {
        public string levelName;
        public string trueSceneName;
        // public int buildIndex;
        public bool containsNavmesh = true;

        public LoadLevelData (string levelName, string trueSceneName, bool containsNavmesh = true)
        {
            levelName = this.levelName;
            trueSceneName = this.trueSceneName;
            containsNavmesh = this.containsNavmesh;
        }

    }

    // Manually define the current scenes
    public LoadLevelData[] allImportantLevelData = new LoadLevelData[]
    {
        new LoadLevelData("town_start", "Town_start", false),
        new LoadLevelData("ow_start", "OW_start_reworked", true),
        new LoadLevelData("town_start_destroyed", "Town_start_destroyed", true),
        new LoadLevelData("ow_start_cave", "Cave_to_Blacksmith", true),
        new LoadLevelData("catacomb_1", "Catacomb_Floor1", true),
        new LoadLevelData("catacomb_2", "Catacomb_Floor2", true),
        new LoadLevelData("catacomb_3", "Catacomb_Floor3", true),
        new LoadLevelData("catacomb_boss", "Catacomb_Floor4", true),
        new LoadLevelData("ow_grasslands", "OW_grasslands", true),
        new LoadLevelData("town_grass", "ow_grass_town", true),
        new LoadLevelData("verdant_castle_boss", "VerdantCastle_Floor5", true),
    };
    private List<string> allLevelKeynames = new List<string>();
    public List<string> AllLevelKeynames { get { return allLevelKeynames;}}
    private List<string> allLevelFullnames = new List<string>();
    public List<string> AllLevelFullnames { get { return allLevelFullnames;}}

    // Prevent the ToSceneCommand from working properly 
    [SerializeField]
    private List<string> blacklistedScenes = new List<string>
    {
        "Title",
        "Initialization"
    };

    // private bool canUseCommands = false;
    private LoadLevelData currentLevelData = null;
    // Will be searched by the ToSceneCommand to check if the inputted string is contained within this dictionary
    public Dictionary<string, LoadLevelData> allLevelData = new Dictionary<string, LoadLevelData>();

    public static SceneDebugManager instance;
    public LoadLevelData CurrentLevel { get { return currentLevelData; } }
    
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; 

        // Initializes the level dictionary to be used by command and VisualStateManager (will have keys for both the abbreviated command and the true case sensitive name)
        foreach(LoadLevelData level in allImportantLevelData)
        {
            if(!allLevelData.ContainsKey(level.levelName))
            {
                allLevelData.Add(level.levelName, level);
                allLevelKeynames.Add(level.levelName);
            }

            if(!allLevelData.ContainsKey(level.trueSceneName))
            {
                allLevelData.Add(level.trueSceneName, level);
                allLevelFullnames.Add(level.trueSceneName);
            }

        }
    }

    private void Start()
    {
        if(LevelManager.levelManager != null)
        {
            LevelManager.levelManager.onNewSceneEvent.AddListener(ObtainCurrentSceneData);
            LevelManager.levelManager.onNewSceneEvent.AddListener(MaintainPlayerInputsDisabled);
        }
        else
        {
            ObtainCurrentSceneData();
        }
        // LevelManager.levelManager.onNewSceneLoadAsyncEvent.AddListener(CheckNavMeshInAsyncScene);
    }

    private void ObtainCurrentSceneData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        LoadLevelData level = allImportantLevelData.FirstOrDefault(levelData => levelData.trueSceneName == sceneName);
        if(level != null)
        {
            currentLevelData = allLevelData[level.levelName];
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"\"{sceneName}\" is not a valid scene to be debugged.");
            currentLevelData = null;
        }
    }
    
    private void MaintainPlayerInputsDisabled()
    {
        if(DeveloperConsoleController.instance.ShowConsole)
        {
            PlayerInputManager.playerInputs.PlayerAction.Disable(); 
            PlayerInputManager.playerInputs.CameraAction.Disable(); 
            PlayerInputManager.playerInputs.AllyAction.Disable();
        }
    }
    // Used by VisualizationStateManager to determine if we need to delete any allies in the player when we transition into a scene wiith no navmesh
    public bool SceneContainsNavmesh(string sceneName)
    {
        if(blacklistedScenes.Contains(sceneName))
        {
            return false;
        }
        LoadLevelData level = allImportantLevelData.FirstOrDefault(levelData => levelData.trueSceneName == sceneName);
        if(level != null)
        {
            return level.containsNavmesh;
        }
        return false;
    }

    public void LoadScene(string sceneName, int spawnIndex)
    {
        if(allLevelData.ContainsKey(sceneName))
        {
            LevelManager.levelManager.LoadLevel(allLevelData[sceneName].trueSceneName, spawnIndex);
            return;
        }
        DeveloperConsoleController.AddStaticMessageToConsole($"\"{sceneName}\" is not a valid scene name.");
    }

    public void LoadNextOrPriorScene(bool isNext, int spawnIndex)
    {
        string nextScene = AcquireNextOrPriorScene(currentLevelData.levelName, isNext);
        if(nextScene != null)
        {
            // Debug.Log($"Scene name to load: {allLevelData[nextScene].trueSceneName} at index: {spawnIndex}");
            LevelManager.levelManager.LoadLevel(allLevelData[nextScene].trueSceneName, spawnIndex);
        }
    }
    // For the ToScene command whenever the player enters "next" or "prior", will search through the list.
    private string AcquireNextOrPriorScene(string sceneName, bool isNext = true) 
    {
        int mainIndex = 0;

        // checks if the sceneName even exists in sceneDictionary
        if(allLevelData.ContainsKey(sceneName))
        {
            Debug.Log("Entered allLevelData loop");
            for(int i = 0; i < allImportantLevelData.Length; i++)
            {
                if(allImportantLevelData[i].levelName == sceneName || allImportantLevelData[i].trueSceneName == sceneName)
                {
                    mainIndex = i;
                    break;
                }
            }
            
            string chosenScene = allImportantLevelData[mainIndex].trueSceneName;
            
            // Check if the mainIndex is over the mi
            switch(isNext)
            {
                case true:
                    mainIndex += 1;
                    if(mainIndex > allImportantLevelData.Length - 1)
                    {
                        return allImportantLevelData[0].trueSceneName;
                    }
                break;
                case false:
                    mainIndex -= 1;
                    if(mainIndex < 0)
                    {
                        return allImportantLevelData[allImportantLevelData.Length - 1].trueSceneName;
                    }
                break;
            }

            return allImportantLevelData[mainIndex].trueSceneName;
        }

        return null;
    }

    private void CheckNavMeshInAsyncScene(string sceneName)
    {
        // Will reference the SceneDebugManager to determine if a scene contains a navmesh
        if(SceneContainsNavmesh(sceneName))
        {
            DestroyAlliesOfAgentID(removeAll: true);
        }
    }

    public void DestroyAlliesOfAgentID(int agentID = 0, bool removeAll = false)
    {
        if(UnitGroupManager.current.allUnits.units.Count > 0)
        {
            // Removes all units from the player's party, then destroys them.
            NPCUnit[] allAllies = UnitGroupManager.current.allUnits.units.ToArray();
            foreach(NPCUnit unit in allAllies)
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
        if(LevelManager.levelManager != null)
        {
            LevelManager.levelManager.onNewSceneEvent.RemoveListener(ObtainCurrentSceneData);
            LevelManager.levelManager.onNewSceneLoadAsyncEvent.RemoveListener(CheckNavMeshInAsyncScene);
        }
        
        // LevelManager.levelManager.onNewSceneEvent.AddListener(MaintainPlayerInputsDisabled);
    }

    
}
