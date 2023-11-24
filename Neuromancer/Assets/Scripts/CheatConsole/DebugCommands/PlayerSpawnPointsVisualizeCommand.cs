using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Spawn Points Visualize Command", menuName = "DeveloperConsole/Commands/Player Spawn Points Visualize Command")]
public class PlayerSpawnPointsVisualizeCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        // needs only 1 argument:
        if(args.Length == 0)
        {
            // if no argument, just do the opposite:
            bool currBool = VisualizationStateManager.instance.ShowPlayerSpawnPointVisuals;
            VisualizationStateManager.instance.togglePlayerSpawnPointVisualizationsEvent?.Invoke(!currBool);
            return true;
        }
        else if(args.Length == 1)
        {
           switch (args[0])
           {
                default:
                case "on":
                case "true":
                    VisualizationStateManager.instance.togglePlayerSpawnPointVisualizationsEvent?.Invoke(true);
                break;

                case "off":
                case "false":
                    VisualizationStateManager.instance.togglePlayerSpawnPointVisualizationsEvent?.Invoke(false);
                break;
           }
           return true;

        }
        
        return false;
    }
}
