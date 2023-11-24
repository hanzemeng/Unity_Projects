using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.AI;
using Neuromancer;
using System.Linq;

[CreateAssetMenu(fileName = "New To Scene Command", menuName = "DeveloperConsole/Commands/To Scene Command")]
public class ToSceneCommand : ConsoleCommand
{
    
    // LevelManager levelManager = LevelManager.levelManager;
    private const string PLAYER_SPAWN_POINTS = "PlayerSpawnPoints";
    
    public override bool Process(string[] args)
    {
        if(args.Contains("--help") && this.usesOwnHelp)
        {
            return CustomHelpCommand();
        }
        if(LevelManager.levelManager == null)
        {
            DeveloperConsoleController.AddStaticMessageToConsole("Cannot use toScene command, level manager does not exist.");
            return false;
        }
        // This command will NOT have more than 2 arguments:
        if (args.Length > 0 && args.Length <= 2)
        {
            int spawnPointIndex = 0;
            if (args.Length == 2)
            {   
                //Debug.Log($"The spawnIndex {args[1]} was successfully parsed!");
                if(int.TryParse(args[1], out spawnPointIndex))
                {
                    spawnPointIndex = int.Parse(args[1]);
                }
            }

            switch (args[0])
            {
                case "this":
                    SpawnInCurrentScene(spawnPointIndex);
                break;
                case "next":
                    SceneDebugManager.instance.LoadNextOrPriorScene(isNext: true, spawnPointIndex);
                break;
                case "prior":
                    SceneDebugManager.instance.LoadNextOrPriorScene(isNext: false, spawnPointIndex);
                break;
                default:
                    if(args[0] == SceneDebugManager.instance.CurrentLevel.levelName || args[0] == SceneDebugManager.instance.CurrentLevel.trueSceneName)
                    {
                        SpawnInCurrentScene(spawnPointIndex);
                    }
                    else
                    {
                        SceneDebugManager.instance.LoadScene(args[0], spawnPointIndex);
                    }
                    
                break;
            }
            return true;
        }
        return false;
    }

    // only apply if the user is issuing the command in the current scene they're in, just skip the loading to make it convenient
    private void SpawnInCurrentScene(int spawnPointIndex)
    {
        GameObject[] playerSpawnPoints = GameObject.Find(PLAYER_SPAWN_POINTS).GetComponent<PlayerSpawnPoints>().playerSpawnPoints;
        // Check if it is a valid spawn point
        spawnPointIndex = spawnPointIndex > playerSpawnPoints.Length - 1 || spawnPointIndex < 0 ? 0 : spawnPointIndex;
        // Debug.Log($"Player's position before warp = (x = {PlayerController.player.transform.position.x} , y = {PlayerController.player.transform.position.y}, z = {PlayerController.player.transform.position.z})");
        PlayerController.player.enabled = false;
        PlayerController.player.GetComponent<CharacterController>().enabled = false;
        PlayerController.player.transform.position = playerSpawnPoints[spawnPointIndex].transform.position;
        // Debug.Log($"Player's position AFTER warp = (x = {PlayerController.player.transform.position.x} , y = {PlayerController.player.transform.position.y}, z = {PlayerController.player.transform.position.z})");
        PlayerController.player.transform.rotation = playerSpawnPoints[spawnPointIndex].transform.localRotation;
        PlayerController.player.GetComponent<CharacterController>().enabled = true;
        PlayerController.player.enabled = true;

        List<NPCUnit> allAllies = UnitGroupManager.current.allUnits.units;

        foreach (NPCUnit ally in allAllies)
        {
            SceneManager.MoveGameObjectToScene(ally.gameObject, SceneManager.GetActiveScene());
            float randomRadian = Random.Range(0f, 2f*Mathf.PI);
            Vector3 randomOffset =  Random.Range(5f, 8f) * new Vector3(Mathf.Cos(randomRadian), 0f, Mathf.Sin(randomRadian));
            Vector3 target = playerSpawnPoints[spawnPointIndex].transform.position + randomOffset;
            ally.GetComponent<NavMeshAgent>().Warp(target);
            ally.IssueCommand(new Command(CommandType.ATTACK_FOLLOW, PlayerController.player.transform));
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

        string keyword = "\tSCENE KEYWORDS:\t\t      TRUE SCENE NAMES:";
        //string keyword = string.Format("\t{0, -35}\t\t{1,-20}", "VALID KEYWORDS:", "TRUE SCENE NAMES:");
        DeveloperConsoleController.AddStaticMessageToConsole(keyword);
        string allDataEntries = "";
        List<string> allKeynames= SceneDebugManager.instance.AllLevelKeynames;
        List<string> allFullnames = SceneDebugManager.instance.AllLevelFullnames;
        for(int i = 0; i < allKeynames.Count; i++)
        {
            //string dataEntry = $"\t    {allKeynames[i], -35}\t\t    {allFullnames[i], 35}";
            string dataEntry = string.Format("\t{0, -35}\t\t{1, -15}", " " + allKeynames[i], " " + allFullnames[i]);
            allDataEntries += dataEntry + "\n";
            continue;
        }

        DeveloperConsoleController.AddStaticMessageToConsole(allDataEntries);
        DeveloperConsoleController.AddStaticMessageToConsole("=========================================================================");
        return true;
    }
}
