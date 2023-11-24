using System.Collections.Generic;
using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField] private string commandWord = string.Empty;
    [SerializeField] public string description = string.Empty;
    [SerializeField] public string help = string.Empty;
    public bool usesOwnHelp = false;

    public string CommandWord => commandWord;

    public string Description => description;
    
    public string Help => help;

    public bool UsesOwnHelp => usesOwnHelp;

    // override process function so we can do whatever we want
    public abstract bool Process(string[] args);
}
