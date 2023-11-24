using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "God Mode Command", menuName = "DeveloperConsole/Commands/God Mode Command")]
public class GodModeCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (PlayerController.player != null)
        {
            if(args.Length == 1)
            {
                switch (args[0])
                {
                    case "true":
                    case "on":
                        PlayerController.player.GodModeEvent?.Invoke(true);
                        DeveloperConsoleController.AddStaticMessageToConsole("God Mode has been enabled");
                    break;
                    case "false":
                    case "off":
                        PlayerController.player.GodModeEvent?.Invoke(false);
                        DeveloperConsoleController.AddStaticMessageToConsole("God Mode has been disabled");
                    break;
                }
                return true;
            }
        }
        return false;
    }
}
