using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[CreateAssetMenu(fileName = "New NavMesh Visual Command", menuName = "DeveloperConsole/Commands/NavMesh Visual Command")]
public class NavmeshVisualCommand : ConsoleCommand
{
    // List of all agentIDs, the last index is simply for the "all" and "none" arguments:
    public readonly Dictionary<string, int> navMeshAgents 
    = new Dictionary<string, int> 
    {
        {"humanoid", 0},
        {"grounded", 0},
        {"flier", -334000983},
        {"flyer", -334000983},
        {"swimmer", 1479372276},
        {"swim", 1479372276},
    };

    // Simple toggle used to determine if the command is being used to show the navmesh when invoked
    [SerializeField] private bool showNavMesh = true;

    public override bool Process(string[] args)
    {
        // needs only 1 argument:
        if(args.Length == 1)
        {
            if(navMeshAgents.ContainsKey(args[0]))
            {
                VisualizationStateManager.instance.toggleNavMeshVisualizationsEvent?.Invoke(navMeshAgents[args[0]], showNavMesh);
            }
            else
            {
                switch(args[0])
                {
                    case "all":
                    case "":
                        foreach(int id in navMeshAgents.Values.Distinct())
                        {
                            VisualizationStateManager.instance.toggleNavMeshVisualizationsEvent?.Invoke(id, showNavMesh);
                        }
                        break;

                    case "none":
                        foreach(int id in navMeshAgents.Values.Distinct())
                        {
                            VisualizationStateManager.instance.toggleNavMeshVisualizationsEvent?.Invoke(id, !showNavMesh);
                        }
                        break;
                    case "off":
                        VisualizationStateManager.instance.allowNavMeshBuildEvent?.Invoke(false);
                        break;
                    case "on":
                        VisualizationStateManager.instance.allowNavMeshBuildEvent?.Invoke(true);
                        break;
                }
            }

        }
        
        return true;
    }

}
