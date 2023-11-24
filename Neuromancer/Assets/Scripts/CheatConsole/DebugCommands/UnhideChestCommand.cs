using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unhide Chest Visualize Command", menuName = "DeveloperConsole/Commands/Unhide Chest Command")]
public class UnhideChestCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        // needs only 1 argument:
        if(args.Length == 0)
        {
            // if no argument, just do the opposite:
            bool currBool = VisualizationStateManager.instance.UnhideChestVisuals;
            VisualizationStateManager.instance.toggleUnhideChestVisualizationEvent?.Invoke(!currBool);
            return true;
        }
        else if(args.Length == 1)
        {
           switch (args[0])
           {
                default:
                case "on":
                case "true":
                    VisualizationStateManager.instance.toggleUnhideChestVisualizationEvent?.Invoke(true);
                break;

                case "off":
                case "false":
                    VisualizationStateManager.instance.toggleUnhideChestVisualizationEvent?.Invoke(false);
                break;
           }
           return true;

        }
        
        return false;
    }
}
