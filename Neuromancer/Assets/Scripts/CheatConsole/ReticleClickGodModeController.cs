using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Neuromancer;
using static Neuromancer.Unit;
using static Interactable;
using static PlayerInteractable;
using System.Linq;

// Will feature its own Developer Console 
[RequireComponent(typeof(DeveloperConsoleController))]
 public class ReticleClickGodModeController : MonoBehaviour
{
    // This component will have its own DeveloperConsole with its own commands separate from the DeveloperConsoleController:
    [SerializeField] private string prefix = string.Empty;
    // Where the user can drag in any command scriptableObjects, completely separate from the DeveloperConsoleCommand:
    [SerializeField] private ConsoleCommand[] clickCommands = new ConsoleCommand[0];

    private DeveloperConsole developerConsole;
    private DeveloperConsole DeveloperConsole 
    {
        get 
        {   
            if (developerConsole != null) { return developerConsole; }
            return developerConsole = new DeveloperConsole(prefix, clickCommands);
        }
    }

    // Needs a reference for whenever the player clicks on the level..
    private PlayerInputs inputs;

    // Create an event to be invoked by other commands 
    [System.Serializable]
    public class ToggleClickCommandEvent : UnityEvent<string, string[]> { }
    public ToggleClickCommandEvent toggleClickCommandEvent = new ToggleClickCommandEvent();
    [SerializeField] private ConsoleCommand resetToDefaultCommand;

    private string resetToDefaultKeyword = "resetCursor";
    public string ResetToDefaultKeyword { get { return resetToDefaultKeyword;}}

    private string emptyClickPrompt = "";

    public static ReticleClickGodModeController instance;
    
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; 
        inputs = PlayerInputManager.playerInputs;
        
        // reset to default keyword grabs the command as a reference
        resetToDefaultKeyword = resetToDefaultCommand.CommandWord;

        // Initializes the empty click command prompt, will be empty for now.
        emptyClickPrompt = "";
        
    }
    
    private void OnEnable() {
        inputs.AllyAction.AllyControl.started += ClickCommand;
        toggleClickCommandEvent.AddListener(UpdateEmptyClickCommand);

    }

    private void OnDisable() {
        inputs.AllyAction.AllyControl.started -= ClickCommand;
        toggleClickCommandEvent.RemoveListener(UpdateEmptyClickCommand);
    }
    
    // Will subscribe to whatever input is being used for selecting units.
    private void ClickCommand(InputAction.CallbackContext ctx) 
    {
        // Debug.Log("The click has been recognized");
        // Acquires reference to hovered gameObject to check if it's null or not.
        GameObject hoveredObject = ReticleController.current.GetTargetObject();

        // Check if current object is empty, then apply empty command if it is loaded:
        if(hoveredObject != null)
        {
            // Check for the following conditionals: there exists an empty click prompt AND is neither unit nor an interactable gameObject AND no allies were selected:
            if
            ( 
                emptyClickPrompt != "" && 
                !IsUnit(hoveredObject.transform) && 
                hoveredObject.tag != INTERACT_TAG && 
                hoveredObject.tag != PlayerInteractable.PLAYER_INTERACTABLE_TAG &&
                UnitGroupManager.current.selectedUnitGroup.units.Count == 0
            )
            {
                Debug.Log("Valid gameObject to initiate empty click command");
                ProcessCommand(emptyClickPrompt);
            }
        }
    }
    private void UpdateEmptyClickCommand(string commandString, string[] commandArgs = null)
    {   
        
        if(commandString == resetToDefaultKeyword)
        {
            ResetAllCommands();
            return;
        }
        string emptyCommand = commandString.Trim();
        if(commandArgs != null || commandArgs.Length != 0)
        {
            emptyCommand += " ";
            emptyCommand += String.Join(" ", commandArgs);
        }
        
        emptyClickPrompt = emptyCommand;
    }


    private void ProcessCommand(string inputValue)
    {
        // Debug.Log($"The string {inputValue} is being processed");
        DeveloperConsole.ProcessCommand(inputValue.Trim());
    }

    // Will reset all commands associated with the reticle.
    private void ResetAllCommands()
    {
        emptyClickPrompt = "";
    }

}
