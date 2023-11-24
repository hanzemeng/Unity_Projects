using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clear Console Command", menuName = "DeveloperConsole/Commands/Clear Console Command")]
public class ClearConsoleCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (args.Length == 0)
        {
            DeveloperConsoleController.instance.ClearConsoleText();
            return true;
        }
        return false;
    }
}
