public interface IConsoleCommand 
{
    string CommandWord { get; }

    string Description { get;}

    string Help {get;}

    bool UsesOwnHelp {get;}
    
    // will support multiple potential arguments
    bool Process(string[] args);
}
