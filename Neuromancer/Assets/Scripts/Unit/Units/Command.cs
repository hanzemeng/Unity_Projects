using UnityEngine;

public enum CommandType
{
    DEFAULT = 0,            // NULL command, tells unit to do default behavior
    IDLE,                   // Command to literally do nothing
    MOVE_TO,                // Command to move to target location (Vector3)
    FLEE_FROM,              // Command to move away from target (transform)
    FOLLOW_TARGET,          // Command to follow a specific target (Transform)
    ATTACK_TARGET,          // Command to attack a specific target (Transform)
    HOLD_POSITION,          // Command to not move, but still allow attacking if in range
    ATTACK_FOLLOW,          // Command to attack the closest enemy in range, OR if no enemies in range, follow specific target (Transform)
                                // If there is no specific target specified, follow player
    ATTACK_MOVE,            // Command to attack the closest enemy in range, OR if no enemies in range, move to target location (Vector3)
    INTERACT,               // Command to move to interactable object and trigger an interaction with the object
    ABILITY_ONE,            // Command to use ability 1
    ABILITY_TWO,            // Command to use ability 2
    ABILITY_THREE,          // Command to use ability 3
}

public enum CommandMode
{
    APPEND,                 // Append command to the end of the Command queue
    REPLACE,                // Replaces entire command queue with the new Command (effectively clears
                            // Command queue before appending the new Command
    PRIORITY,               // Inserts command to the front of the Command queue
}

[System.Serializable]
public class Command
{
    [field: SerializeField]
    public CommandType commandType { get; private set; } = CommandType.DEFAULT;

    public Vector3 startingPosition { get; private set; } = Vector3.zero;

    [field: SerializeField]
    public Vector3 targetPosition { get; private set; } = Vector3.zero;

    [field: SerializeField]
    public Transform targetUnit { get; private set; } = null;

    [field: SerializeField]
    public GameObject targetData { get; private set; } = null;

    public Command()
    {
        // Default constructor sets the commandType to CommandType.DEFAULT
    }

    // Used when creating CommandType.ATTACK_CLOSEST or CommandType.ATTACK_FOLLOW
    public Command(CommandType commandType)
    {
        this.commandType = commandType;
    }

    // Used when creating CommandType.MOVE_TO, CommandType.HOLD_POSITION, or CommandType.ATTACK_MOVE
    public Command(CommandType commandType, Vector3 targetPosition)
    {
        this.commandType = commandType;
        this.targetPosition = targetPosition;
    }

    // Used when creating CommandType.ATTACK_TARGET or CommandType.INTERACT
    public Command(CommandType commandType, Transform targetUnit)
    {
        this.commandType = commandType;
        this.targetUnit = targetUnit;
    }

    // Used when creating CommandType.ABILITY_ONE, CommandType.ABILITY_TWO, or CommandType.ABILITY_THREE
    public Command(CommandType commandType, GameObject targetData, Vector3 targetPosition)
    {
        this.commandType = commandType;
        this.targetData = targetData;
        this.targetPosition = targetPosition;
    }
}