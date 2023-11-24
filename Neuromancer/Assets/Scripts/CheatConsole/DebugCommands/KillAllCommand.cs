using System.Collections;
using System.Collections.Generic;
using Neuromancer;
using UnityEngine;
using EmeraldAI;
using UnityEngine.UIElements;

// Will kill every unit in scene if doable
[CreateAssetMenu(fileName = "Kill All Command", menuName = "DeveloperConsole/Commands/Kill All Command")]
public class KillAllCommand : ConsoleCommand
{
    [SerializeField] private int killDamage = 999999;
    public override bool Process(string[] args)
    {
        if (args.Length <= 1)
        {
            // acquires all current units in the scene:
            NPCUnit[] allUnits = FindObjectsByType<NPCUnit>(FindObjectsSortMode.None);

            if(allUnits.Length > 0)
            {
                foreach(NPCUnit unit in allUnits)
                {
                    if(args.Length < 1)
                    {
                        unit.GetComponent<EmeraldAISystem>().Damage(killDamage, EmeraldAISystem.TargetType.AI, unit.transform, 100);
                    }
                    else 
                    {
                        switch(args[0])
                        {
                            case "enemy":
                            case "enemies":
                                if (Unit.IsEnemy(unit.transform))
                                {
                                    unit.GetComponent<EmeraldAISystem>().Damage(killDamage, EmeraldAISystem.TargetType.AI, unit.transform, 100);
                                }                      
                            break;

                            case "allies":
                            case "ally":
                                if (Unit.IsAlly(unit.transform))
                                {
                                    unit.GetComponent<EmeraldAISystem>().Damage(killDamage, EmeraldAISystem.TargetType.AI, unit.transform, 100);
                                }        
                            break;
                        }
                    }
                }

                return true;
            }
            else
            {
                DeveloperConsoleController.AddStaticMessageToConsole("No units found.");
                return false;
            }
            

        }
        return false;
    }
}
