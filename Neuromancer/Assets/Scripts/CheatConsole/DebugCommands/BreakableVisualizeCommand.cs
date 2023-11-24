using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Breakable Visualize Command", menuName = "DeveloperConsole/Commands/Breakable Visualize Command")]
public class BreakableVisualizeCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        // needs only 1 argument:
        if(args.Length == 0)
        {
            // if no argument, just do the opposite:
            bool currBool = VisualizationStateManager.instance.ShowBreakableVisuals;
            VisualizationStateManager.instance.toggleBreakableVisualizationsEvent?.Invoke(!currBool);
            return true;
        }
        else if(args.Length == 1)
        {
           switch (args[0])
           {
                default:
                case "on":
                case "true":
                    VisualizationStateManager.instance.toggleBreakableVisualizationsEvent?.Invoke(true);
                break;

                case "off":
                case "false":
                    VisualizationStateManager.instance.toggleBreakableVisualizationsEvent?.Invoke(false);
                break;
           }
           return true;

        }
        
        return false;
    }
}
