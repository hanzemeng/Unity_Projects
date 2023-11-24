using UnityEngine;

[CreateAssetMenu(fileName = "New Log Command", menuName = "DeveloperConsole/Commands/Log Command")]
public class LogCommand : ConsoleCommand
{
    private bool consoleOn = false;

    public override bool Process(string[] args)
    {
        switch(args[0])
        {
            case "on":
            case "true":
                consoleOn = true;
                Debug.Log("The Unity Console has been enabled. All log/error/warning messages will be displayed in the Developer Console.");
                break;
            
            case "off":
            case "false":
                Debug.Log("The Unity Console has been disabled.");
                consoleOn = false;
                break;
        }

        return true;
    }

    private void OnEnable() 
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable() 
    {
        Application.logMessageReceived -= HandleLog;
    }

    // Turns on the console log so that it displays all messages inputted in log
    private void HandleLog(string logMessage, string stackTrace, LogType type)
    {
        if(consoleOn)
        {
            string message = "[" + type.ToString() + "] " + logMessage;
            DeveloperConsoleController.AddStaticMessageToConsole(message);
        }

    }


}
