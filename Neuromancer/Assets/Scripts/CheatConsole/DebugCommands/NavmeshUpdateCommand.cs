using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Navmesh Update Command", menuName = "DeveloperConsole/Commands/NavMesh Update Command")]
public class NavmeshUpdateCommand : ConsoleCommand
{
    [SerializeField] private float navMeshUpdateDelayTime = 1f;

    // Simply just use the command's word to update the navmesh visualization.
    public override bool Process(string[] args)
    {
        if(args.Length == 0)
        {
            VisualizationStateManager.instance.updateNavMeshVisualizationsEvent?.Invoke(navMeshUpdateDelayTime);
            return true;
        }
        return false;
    }

}
