using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeColor // r, g, b, a
{
    GREEN = 0x00FF00FF,
    RED = unchecked((int)0xFF0000FF),
    ORANGE = unchecked((int)0xFF8000FF),
    YELLOW = unchecked((int)0xFFFF00FF),
    CYAN = unchecked((int)0x00FFFFFF),
}

public static class NodeColorUtility
{
    public const int NODE_COLOR_COUNT = 5;

    public static Color NodeColorToColor(NodeColor nodeColor)
    {
        float r, g, b, a;

        uint mask = 0xFF000000;

        r = (float)(((int)nodeColor & mask) >> 24) / 256f;
        mask >>= 8;
        g = (float)(((int)nodeColor & mask) >> 16) / 256f;
        mask >>= 8;
        b = (float)(((int)nodeColor & mask) >> 8) / 256f;
        mask >>= 8;
        a = (float)(((int)nodeColor & mask) >> 0) / 256f;

        return new Color(r, g, b, a);
    }

    public static Color IntToColor(int nodeColorIndex)
    {
        if(nodeColorIndex<0 || nodeColorIndex>=NODE_COLOR_COUNT)
        {
            Debug.LogError($"{nodeColorIndex} does not map to any node color");
            return new Color(0f,0f,0f,0f);
        }

        switch(nodeColorIndex)
        {
            case 0:
            {
                 return NodeColorToColor(NodeColor.GREEN);
            }
            case 1:
            {
                 return NodeColorToColor(NodeColor.RED);
            }
            case 2:
            {
                 return NodeColorToColor(NodeColor.ORANGE);
            }
            case 3:
            {
                 return NodeColorToColor(NodeColor.YELLOW);
            }
            case 4:
            {
                 return NodeColorToColor(NodeColor.CYAN);
            }
        }
        return new Color(0f,0f,0f,0f);
    }
}