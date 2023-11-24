using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "New Reticle Click Spawn Object Command", menuName = "DeveloperConsole/Commands/Reticle Click Spawn Object Command")]
public class ReticleClickSpawnInitializeCommand : ConsoleCommand
{
    [SerializeField] private ConsoleCommand associatedCommand;  // the associated command's keyword will be used if necessary, if null use current command's keyword.

    [Header("Prefabs Resource Folder Location & Default Settings")]
    [Tooltip("Pulls from Resouces - make sure the folder name is correct")]
    [SerializeField] private string folderName = "EmeraldAIUnits";
    [Tooltip("A gameObject's name that will serve as the default if no argument is provided. Leave empty if unecessary.")]
    [SerializeField] private string defaultMainKeyword = "";
   
    private List<string> filterWords = new List<string>();

    public override bool Process(string[] args)
    {
        // Want the command to print all keywords for every gameObject.
        if(args.Contains("--help") && this.usesOwnHelp)
        {
            return CustomHelpCommand();
        }

        if (ReticleClickGodModeController.instance != null)
        {
            // Just to double check if the folder name is valid
            if(ResourceDatabaseController.instance != null && ResourceDatabaseController.instance.AllResourceData.ContainsKey(folderName))
            {
                // checks if the serializable list containing all the filter words is empty. If empty, set it to an empty string.
                filterWords = ResourceDatabaseController.instance.ReturnValidFilterWords(folderName);
                string defaultFilter = filterWords.Count > 0 ? filterWords[0] : "";
                string defaultKeyword = defaultMainKeyword != "" ? defaultMainKeyword : ResourceDatabaseController.instance.AllResourceData[folderName].ToList()[0].keyword;

                Object objectToSpawn = null;
                
                switch (args.Length)
                {
                    case 0:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
                        break;
                    case 1:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, args[0], defaultFilter);
                        break;
                    case 2:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, args[0], args[1]);;
                        break;
                    default:
                        objectToSpawn = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
                        break;
                }

                if(objectToSpawn != null)
                {
                    string currentComandWord = associatedCommand != null ? associatedCommand.CommandWord : this.CommandWord;
                    ReticleClickGodModeController.instance.toggleClickCommandEvent?.Invoke(currentComandWord, args);
                    DeveloperConsoleController.AddStaticMessageToConsole($"{objectToSpawn.name} will be spawned anywhere you click on the level now! Type \"{ReticleClickGodModeController.instance.ResetToDefaultKeyword}\" in the console to reset reticle back to normal.");
                    return true;
                }
                else
                {
                    DeveloperConsoleController.AddStaticMessageToConsole($"{args[0]} is not a valid object.");
                    return false;
                }
            }
        }
        return false;
    }

    private bool CustomHelpCommand()
    {
        DeveloperConsoleController.AddStaticMessageToConsole("=========================================================================");
        string fixedDesc = description.Replace("\\n", "\n");
        DeveloperConsoleController.AddStaticMessageToConsole(fixedDesc);
        DeveloperConsoleController.AddStaticMessageToConsole("-------------------------------------------------------------------------");
        string fixedHelp = help.Replace("\\n", "\n");
        DeveloperConsoleController.AddStaticMessageToConsole(fixedHelp);
        // Acquires a reference to the list containing all unit entries
        List<ResourceData> filenameDataList = ResourceDatabaseController.instance.AllResourceData[folderName].ToList();
        List<string> allKeywords = new List<string>();
        string keyword = "    ALL VALID KEYWORDS:";
        DeveloperConsoleController.AddStaticMessageToConsole(keyword.PadLeft(3));
        string allDataEntries = "";
        foreach(ResourceData data in filenameDataList)
        {
            if(!allKeywords.Contains(data.keyword))
            {
                string dataEntry = $"\t{data.keyword}";
                allDataEntries += dataEntry + "\n";
                allKeywords.Add(data.keyword);
                continue;
            }          
        }
        DeveloperConsoleController.AddStaticMessageToConsole(allDataEntries);

        filterWords = ResourceDatabaseController.instance.ReturnValidFilterWords(folderName);
        string defaultFilter = filterWords.Count > 0 ? filterWords[0] : "";
        Object defaultObject = ResourceDatabaseController.instance.FindObject(folderName, defaultMainKeyword, defaultFilter);
        if(filterWords.Count > 0)
        {
            DeveloperConsoleController.AddStaticMessageToConsole($"The default filter used is \"{defaultFilter}\" if no second argument is provided.");
        }
        DeveloperConsoleController.AddStaticMessageToConsole($"The default object is \"{defaultObject.name}\" if no parameters are given.");
        DeveloperConsoleController.AddStaticMessageToConsole("=========================================================================");
        return true;
    }

}
