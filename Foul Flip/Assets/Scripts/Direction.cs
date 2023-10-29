using System.Collections.Generic;

public enum Direction
{
    UP = 0x00000001,
    UP_LEFT = 0x00000002,
    LEFT = 0x00000004,
    LEFT_DOWN = 0x00000008,
    DOWN = 0x00000010,
    DOWN_RIGHT = 0x00000020,
    RIGHT = 0x00000040,
    RIGHT_UP = 0x00000080,
    CENTER = 0x00000100,
}

public static class DirectionUtility
{
    public const int DIRECTION_COUNT = 9;

    public static (int, int) DirectionToOffset(Direction direction) // Item1 is vertical, Item2 is horizontal; down right is positive
    {
        switch(direction)
        {
            case Direction.UP:
            {
                return (-1, 0);
            }
            case Direction.UP_LEFT:
            {
                return (-1, -1);
            }
            case Direction.LEFT:
            {
                return (0, -1);
            }
            case Direction.LEFT_DOWN:
            {
                return (1, -1);
            }
            case Direction.DOWN:
            {
                return (1, 0);
            }
            case Direction.DOWN_RIGHT:
            {
                return (1, 1);
            }
            case Direction.RIGHT:
            {
                return (0, 1);
            }
            case Direction.RIGHT_UP:
            {
                return (-1, 1);
            }
            case Direction.CENTER:
            {
                return (0, 0);
            }
        }
        return (-1, -1);
    }

    public static List<(int, int)> DirectionIntToOffsets(int directionInt)
    {
        List<(int, int)> res = new List<(int, int)>();

        int mask = 0x00000001;
        for(int i=0; i<DIRECTION_COUNT; i++)
        {
            if(0 != (mask & directionInt))
            {
                res.Add(DirectionToOffset((Direction)mask));
            }
            mask <<= 1;
        }
        return res;
    }
}