using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

// Does all the high level stuff for the DebugConsole
public class DeveloperConsoleController : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    // Where the user can drag in any command scriptableObjects
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI Components for Debugger")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private string initializedText;
    [SerializeField] private ScrollRect scrollBar;  // force all commands to go to the bottom
    [SerializeField] private TMP_Text consoleText;  // will be used to record history of console commands

    // pause when in games
    private PlayerInputs inputs;
    private bool showConsole;
    public bool ShowConsole { get { return showConsole; } }
    private Queue<string> previousInputtedStrings = new Queue<string>();
    private int currentIndex= 0;
    [SerializeField] private int previousInputtedStringsCap = 5;

    // Should only ever have ONE developer console - hence, the Singleton pattern
    public static DeveloperConsoleController instance;
    private DeveloperConsole developerConsole;
    private DeveloperConsole DeveloperConsole 
    {
        get 
        {   
            if (developerConsole != null) { return developerConsole; }
            return developerConsole = new DeveloperConsole(prefix, commands);
        }
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this; 

        // grabs PlayerInput singleton
        inputs = PlayerInputManager.playerInputs;
        inputs.CheatConsole.Enable();
        showConsole = false;

        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }

    private void Start()
    {
        uiCanvas.SetActive(false);
        if(initializedText != "")
        {
            ProcessCommand(initializedText);
        }
    }

    private void OnEnable() {
        inputs.CheatConsole.ToggleDebug.performed += OnToggleDebug;
        inputs.CheatConsole.Enter.performed += OnReturn;
        inputs.CheatConsole.CommandNavigation.performed += CommandNavigation;
    }

    private void OnDisable() {
        inputs.CheatConsole.ToggleDebug.performed -= OnToggleDebug;
        inputs.CheatConsole.Enter.performed -= OnReturn;
        inputs.CheatConsole.CommandNavigation.performed -= CommandNavigation;
    }

    public void OnToggleDebug(InputAction.CallbackContext callbackContext)
    {
        showConsole = !showConsole;
        if(showConsole) 
        {
            uiCanvas.SetActive(true); 
            inputField.ActivateInputField();    // get it ready to type - dont have to move mouse to console box
            inputs.PlayerAction.Disable(); 
            inputs.CameraAction.Disable(); 
            inputs.AllyAction.Disable();

            // Time.timeScale = 0;
        } 
        else 
        {
            uiCanvas.SetActive(false);
            inputs.PlayerAction.Enable(); 
            inputs.CameraAction.Enable();
            inputs.AllyAction.Enable();

            // Time.timeScale = pausedTimeScale
        }
    }

    // Will be called by the UI Canvas
    public void ProcessCommand(string inputValue)
    {

        AddMessageToConsole(inputField.text);
        AddToPreviouslyInputtedCommands(inputValue);
        DeveloperConsole.ProcessCommand(inputValue.Trim());
        inputField.text = string.Empty;
        currentIndex = 0;
        inputField.ActivateInputField();
    }

    public void OnReturn(InputAction.CallbackContext callbackContext)
    {
        if(inputField.text != "")
        {
            ProcessCommand(inputField.text);
            //AddMessageToConsole(inputField.text);
        }
    }

    private void AddMessageToConsole(string message)
    {
        consoleText.text += "\n" + message;         // takes whatever new message we have, and slaps a new line
    }

    // Called by other methods outside of the static message (NOTE: these wont be recorded in a list where the player can recall them with UP and DOWN on keyboard)
    public static void AddStaticMessageToConsole(string message)
    {
        DeveloperConsoleController.instance.consoleText.text += "\n" + message;
        DeveloperConsoleController.instance.scrollBar.verticalNormalizedPosition = 0f;
    }

    public void ClearConsoleText()
    {
        consoleText.text = "";
    }

    private void AddToPreviouslyInputtedCommands(string inputValue)
    {
        if(previousInputtedStrings.Count == previousInputtedStringsCap)
        {
            previousInputtedStrings.Dequeue();
        }
        previousInputtedStrings.Enqueue(inputValue);
    }

    private void CommandNavigation(InputAction.CallbackContext callbackContext)
    {
        Vector2 inputVector = callbackContext.ReadValue<Vector2>();
        if(previousInputtedStrings.Count > 0)
        {
            if(inputVector.y < 0)
            {
                currentIndex = (currentIndex + 1) % previousInputtedStrings.Count;
            }
            else if (inputVector.y > 0)
            {
                currentIndex = (currentIndex - 1 + previousInputtedStrings.Count) % previousInputtedStrings.Count;
                StartCoroutine(ResetCursorToEndOfLine());
            }

            string[] arrayCopy = previousInputtedStrings.ToArray();
            inputField.text = arrayCopy[currentIndex];
        }

    }

    // after putting console text, forces scrollRect to go to bottom
    public IEnumerator ResetCursorToEndOfLine()
    {
        yield return new WaitForEndOfFrame();
        inputField.MoveToEndOfLine(false, true);
    }
}
