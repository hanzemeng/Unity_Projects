using System;
using System.Collections.Generic;
using System.Linq;

public class DeveloperConsole
{
    private readonly string prefix;
    private readonly IEnumerable<IConsoleCommand> commands;
    public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
    {
        this.prefix = prefix;
        this.commands = commands;
    }

    // The main ProcessCommand method used by the Debug UI
    public void ProcessCommand(string inputValue)
    {
        // if the input does NOT start with a valid prefix -> skip
        if(!inputValue.StartsWith(prefix)) { return; }

        // Get rid of the prefix in the command starting at INDEX 0
        inputValue = inputValue.Remove(0, prefix.Length);

        // Splits value based on space
        string[] inputSplit = inputValue.Split(' ');    // all I'm left with here is the COMMAND word and the argument(s)

        // The first word will ALWAYS be the command word
        string commandInput = inputSplit[0];
        
        // String array which COULD be empty, but if isnt grabs all the arguments
        string[] args = inputSplit.Skip(1).ToArray();

        if(commandInput == "--help" && args.Length <= 0)
        { 
            DeveloperConsoleController.AddStaticMessageToConsole("===============================================");
            DeveloperConsoleController.AddStaticMessageToConsole("\tALL VALID COMMANDS TO INPUT:\n");
            foreach(IConsoleCommand command in commands)
            {
                string commandDesc = command.Description.Split("\\n")[0]; // only acquire the first sentence description before a new line.
                string commandEntry = "";
                commandEntry = string.Format("{0, -35}\t{1}", command.CommandWord, commandDesc);
                // Hacky solution to two weird outliers to weird off-looking exceptions for these two specific commands.
                if(command.CommandWord == "log" || command.CommandWord == "killAll" || command.CommandWord == "clear")
                {
                    commandEntry = string.Format("{0, -35}\t\t{1}", command.CommandWord, commandDesc);
                }
                DeveloperConsoleController.AddStaticMessageToConsole(commandEntry);
            }
            DeveloperConsoleController.AddStaticMessageToConsole("\nType \"--help\" as an argument for a command to learn more information.");
            DeveloperConsoleController.AddStaticMessageToConsole("===============================================");
            return;
        }
 
        ProcessCommand(commandInput, args);
    }

    public void ProcessCommand(string commandInput, string[] args)
    {
        // Go through all the registered commands...
        foreach(var command in commands)
        {
            // if the current command doe NOT match requested command
            // Equals will make sure it ignores case sensitivity.
            if(!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            else
            {
                // Some helper functions will have to be defined in the command
                if(args.Contains("--help") && !command.UsesOwnHelp)
                {
                    DeveloperConsoleController.AddStaticMessageToConsole("===============================================");
                    string fixedDesc = command.Description.Replace("\\n", "\n");
                    DeveloperConsoleController.AddStaticMessageToConsole(fixedDesc);
                    DeveloperConsoleController.AddStaticMessageToConsole("-----------------------------------------------");
                    string fixedHelp = command.Help.Replace("\\n", "\n");
                    DeveloperConsoleController.AddStaticMessageToConsole(fixedHelp);
                    DeveloperConsoleController.AddStaticMessageToConsole("===============================================");
                    return;
                }
            }

            // The Process method will happen based on how its defined in the command.
            if (command.Process(args))
            {
                return;
            }
        }
    }

 
}
