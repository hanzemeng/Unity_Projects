using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;
using Neuromancer;
using static Neuromancer.Unit;


[CreateAssetMenu(fileName = "New Spawn GameObject Command", menuName = "DeveloperConsole/Commands/Spawn GameObject Command")]
public class SpawnGameObjectCommand : ConsoleCommand
{
    [Header("Prefabs Resource Folder Location & Default Settings")]
    [Tooltip("Pulls from Resouces - make sure the folder name is correct")]
    [SerializeField] private string folderName = "EmeraldAIUnits";
    // [Tooltip("Aside from the main word, this list of keywords will be used in conjunction when searching for a specific object.\n(can be left empty if not necessary)")]
    // [SerializeField] private List<string> filterWords = new List<string>();
    [Tooltip("A gmeObject's name that will serve as the default if no argument is provided. Leave empty if unecessary.")]
    [SerializeField] private string defaultMainKeyword = "";
    
    [Header("GameObject Instantiation on NavMesh Settings")]
    [Tooltip("The maximum spawn radius the command will sample a random spawn point on the navmesh.\nUses the player's current transform as the center by default.")]
    [SerializeField, Range(0.5f, 5f)] private float minSpawnRadius = 1.5f;
    [Tooltip("The maximum spawn radius the command will sample a random spawn point on the navmesh.\nUses the player's current transform as the center by default.")]
    [SerializeField, Range(0.5f, 5f)] private float maxSpawnRadius = 2f;
    [Tooltip("The number of iterations to take in sampling from the current scene's navmesh to find a valid location.")]
    [SerializeField, Range(1, 10)] private int numberOfIterationsForNavmeshSearch = 5;
    [Tooltip("The number of iterations to find a random point between the minimum radius and maximum radius.")]
    [SerializeField, Range(10, 100)] private int maxItertionsForRandomPoint = 100;
    [Tooltip("Enable true ONLY IF the objects to spawn have an NPCUnit component attached to them.")]
    [SerializeField] private bool isUnit = false;
    [Tooltip("Enable true ONLY IF the objects will be spawned using the reticle's current position.")]
    [SerializeField] private bool isInstantiateOnClick = false;
    
    private List<string> filterWords = new List<string>();

    public override bool Process(string[] args)
    {
        // Want the command to print all keywords for every gameObject.
        if(args.Contains("--help") && this.usesOwnHelp)
        {
            return CustomHelpCommand();
        }

        // Make sure the NavMesh surface exists in the current scene:
        if(NavMeshSurfaceGenerator.current != null && NavMeshSurfaceGenerator.current.Surfaces.Length > 0)
        {
            // Just to double check if the folder name is valid
            if(ResourceDatabaseController.instance != null && ResourceDatabaseController.instance.AllResourceData.ContainsKey(folderName))
            {
                // checks if the serializable list containing all the filter words is empty. If empty, set it to an empty string.
                filterWords = ResourceDatabaseController.instance.ReturnValidFilterWords(folderName);
                string defaultFilter = filterWords.Count > 0 ? filterWords[0] : "";
                string defaultKeyword = defaultMainKeyword != "" ? defaultMainKeyword : ResourceDatabaseController.instance.AllResourceData[folderName].ToList()[0].keyword;

                Object objectToSpawn = null;
                
                switch (args.Length)
                {
                    case 0:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
                        break;
                    case 1:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, args[0], defaultFilter);
                        break;
                    case 2:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, args[0], args[1]);;
                        break;
                    default:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
                        break;
                }

                if(objectToSpawn != null)
                {
                    // by default, will instantiate based on player's current position.
                    // Will return a target position based on set booleans
                    Vector3 targetPosition = isInstantiateOnClick ? ReticleController.current.GetReticleTransform().position : PlayerController.player.transform.position;
                    return InstantiateGameObjectInNavMesh(objectToSpawn, targetPosition, isUnit);
                }
                else
                {
                    DeveloperConsoleController.AddStaticMessageToConsole($"{args[0]} is not a valid object.");
                    return false;
                }
            }
            else
            {
                DeveloperConsoleController.AddStaticMessageToConsole($"Resources does NOT have a valid folder with the directory path {folderName}");
                return false;
            } 
         
        }
        else
        {
            DeveloperConsoleController.AddStaticMessageToConsole("The current scene does not have any valid navmeshes, cannot spawn gameObject!");
        }

        return false;
    }

    // Use to instantiate gameObject in current scene's navmesh
    private bool InstantiateGameObjectInNavMesh(Object target, Vector3 targetCenter, bool isUnit)
    {
        
        int navMeshIndex = NavMesh.AllAreas;
        for(int i = 0; i < numberOfIterationsForNavmeshSearch; i++)
        {
            Vector3 randomPoint = GenerateRandomPoint(targetCenter, minSpawnRadius, maxSpawnRadius);
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, navMeshIndex))
            {
                if(isUnit)
                {
                    HandleUnitInstantiationCase(target, hit.position);
                }
                else
                {
                    GameObject.Instantiate((GameObject)target, hit.position, Quaternion.identity);
                    DeveloperConsoleController.AddStaticMessageToConsole($"{target.name} has been spawned!");
                }
                
                return true;
            }
        }
        DeveloperConsoleController.AddStaticMessageToConsole($"Unable to find a valid navmesh location to spawn {target.name}");
        return false;
    }

    // method used in case the desired GameObject prefab contains the NPC Unit component.
    private void HandleUnitInstantiationCase(Object unit, Vector3 targetPosition)
    {
        // First, instantiate the gameObject from Resources 
        GameObject targetObject = GameObject.Instantiate((GameObject)unit, targetPosition, Quaternion.identity);
        
        NPCUnit objectNPCunit = null;
        
        if(CheckUnitOnValidNavmeshArea(targetObject))
        {
            // Next, check that the unit is an ally AND the player's current ally count is low enough.
            switch (IsAlly(targetObject.transform))
            {
                case true:
                    if(UnitGroupManager.current.allUnits.units.Count < UnitGroupManager.current.maxAllies)
                    {
                        DeveloperConsoleController.AddStaticMessageToConsole($"Spawned {targetObject.name} successfully");
                        // targetObject = Instantiate(unit, targetPosition - new Vector3(0, objectNPCunit.EmeraldComponent.m_NavMeshAgent.baseOffset, 0), unit.transform.rotation);
                        objectNPCunit = targetObject.GetComponent<NPCUnit>();
                        UnitGroupManager.current.AddUnit(objectNPCunit);
                        //allyNPCunit.onConvertToAlly?.Invoke();
                    }
                    else
                    {
                        Destroy(targetObject);
                        DeveloperConsoleController.AddStaticMessageToConsole("Unable to spawn ally unit, player's party size is at the maximum already.");
                    }
                    break;
                case false:
                    DeveloperConsoleController.AddStaticMessageToConsole($"Spawned {targetObject.name} successfully");
                    // targetObject = Instantiate(unit, targetPosition - new Vector3(0, objectNPCunit.EmeraldComponent.m_NavMeshAgent.baseOffset, 0), unit.transform.rotation);
                    break;
            }
        }
        else
        {
            Destroy(targetObject);
        }
        
    }

    private Vector3 GenerateRandomPoint(Vector3 center,float innerSpawnRadius, float outerSpawnRadius)
    {
        Vector3 randomPoint = center;
        if(innerSpawnRadius < outerSpawnRadius)
        {
            for(int i = 0; i < maxItertionsForRandomPoint; i++)
            {
                randomPoint = center + Random.insideUnitSphere * outerSpawnRadius;

                if(randomPoint.magnitude >= innerSpawnRadius)
                {
                    // Debug.Log($"random point found at: ({randomPoint.x}, {randomPoint.y}, {randomPoint.z})" );
                    return randomPoint;
                }
            }
        }
        // Debug.Log("No valid point found.");
        return center;
    }
    
    private bool CheckUnitOnValidNavmeshArea(GameObject unit)
    {
        // yield return new WaitForEndOfFrame();
        if(!unit.GetComponent<NavMeshAgent>().isOnNavMesh)
        {   
            DeveloperConsoleController.AddStaticMessageToConsole($"{unit.name} was unable to be created! Its agent type is NOT compatible with chosen spawn location.");
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CustomHelpCommand()
    {
        DeveloperConsoleController.AddStaticMessageToConsole("=========================================================================");
        string fixedDesc = description.Replace("\\n", "\n");
        DeveloperConsoleController.AddStaticMessageToConsole(fixedDesc);
        DeveloperConsoleController.AddStaticMessageToConsole("-------------------------------------------------------------------------");
        string fixedHelp = help.Replace("\\n", "\n");
        DeveloperConsoleController.AddStaticMessageToConsole(fixedHelp);
        // Acquires a reference to the list containing all unit entries
        List<ResourceData> filenameDataList = ResourceDatabaseController.instance.AllResourceData[folderName].ToList();
        List<string> allKeywords = new List<string>();
        string keyword = "    ALL VALID KEYWORDS:";
        DeveloperConsoleController.AddStaticMessageToConsole(keyword.PadLeft(3));
        string allDataEntries = "";
        foreach(ResourceData data in filenameDataList)
        {
            if(!allKeywords.Contains(data.keyword))
            {
                string dataEntry = $"\t{data.keyword}";
                allDataEntries += dataEntry + "\n";
                allKeywords.Add(data.keyword);
                continue;
            }          
        }
        DeveloperConsoleController.AddStaticMessageToConsole(allDataEntries);

        filterWords = ResourceDatabaseController.instance.ReturnValidFilterWords(folderName);
        string defaultFilter = filterWords.Count > 0 ? filterWords[0] : "";
        Object defaultObject = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
        if(filterWords.Count > 0)
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"The default filter used is \"{defaultFilter}\" if no second argument is provided.");
        }
        DeveloperConsoleController.AddStaticMessageToConsole($"The default object is \"{defaultObject.name}\" if no parameters are given.");
        DeveloperConsoleController.AddStaticMessageToConsole("=========================================================================");
        return true;
    }

}
