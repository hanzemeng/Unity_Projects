using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Spawner Visualize Command", menuName = "DeveloperConsole/Commands/Unit Spawner Visualize Command")]
public class UnitSpawnerVisualizeCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        // needs only 1 argument:
        if(args.Length == 0)
        {
            // if no argument, just do the opposite:
            bool currBool = VisualizationStateManager.instance.ShowUnitSpawnerVisuals;
            VisualizationStateManager.instance.toggleUnitSpawnerVisualizationEvent?.Invoke(!currBool);
            return true;
        }
        else if(args.Length == 1)
        {
           switch (args[0])
           {
                default:
                case "on":
                case "true":
                    VisualizationStateManager.instance.toggleUnitSpawnerVisualizationEvent?.Invoke(true);
                break;

                case "off":
                case "false":
                    VisualizationStateManager.instance.toggleUnitSpawnerVisualizationEvent?.Invoke(false);
                break;
           }
           return true;

        }
        
        return false;
    }
}
