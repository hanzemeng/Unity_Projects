using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reset Reticle to Default Command", menuName = "DeveloperConsole/Commands/Reset Reticle to Default Command")]
public class ResetReticleToDefaultCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if(ReticleClickGodModeController.instance != null)
        {
            ReticleClickGodModeController.instance.toggleClickCommandEvent?.Invoke(this.CommandWord, args);
            DeveloperConsoleController.AddStaticMessageToConsole("Reticle has been reset to default behavior");
            return true;
        }
        return false;
    }
}
