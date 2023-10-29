using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LEVEL_INFORMATION", menuName = "ScriptableObjects/Level Information", order = 0)]
public class LevelInformation : ScriptableObject
{
    public int dimension; // always dimension * dimension
    public int range; // node value can be [0, range)
    public int seed; // [0, TWO_BILLION)
    public List<int> directionsInts;
    public List<int> nodesValues;

    public int hasWon; // 1 means true, 0 means false

    public LevelInformation()
    {
        directionsInts = new List<int>();
        nodesValues = new List<int>();
    }

    private void OnValidate()
    {
        if(range<=0 || range>NodeColorUtility.NODE_COLOR_COUNT)
        {
            Debug.LogError($"In {name}, range of {range} is out of (0, {NodeColorUtility.NODE_COLOR_COUNT}]");
            return;
        }
        if(dimension*dimension != nodesValues.Count)
        {
            Debug.LogError($"In {name}, Nodes Values mismatches dimension");
            return;
        }

        for(int i=0; i<dimension*dimension; i++)
        {
            if(nodesValues[i]<0 || nodesValues[i]>=range)
            {
                Debug.LogError($"In {name}, Nodes Values at index {i} is out of [0, {range})");
                return;
            }
        }

        Debug.Log($"{name} LGTM");
    }

    public LevelInformation DeepCopy()
    {
        LevelInformation newLevelInformation = CreateInstance<LevelInformation>();

        newLevelInformation.dimension = dimension;
        newLevelInformation.range = range;
        newLevelInformation.seed = seed;

        newLevelInformation.directionsInts = new List<int>();
        foreach(int directionsInt in directionsInts)
        {
            newLevelInformation.directionsInts.Add(directionsInt);
        }

        newLevelInformation.nodesValues = new List<int>();
        foreach(int nodeValue in nodesValues)
        {
            newLevelInformation.nodesValues.Add(nodeValue);
        }

        newLevelInformation.hasWon = hasWon;

        return newLevelInformation;
    }

    public void Randomize()
    {
        System.Random random = new System.Random(seed);

        hasWon = 0;
        directionsInts = new List<int>();
        nodesValues = new List<int>();

        int upperLimit = 0x00000001<<(DirectionUtility.DIRECTION_COUNT-1);
        for(int i=0; i<dimension*dimension; i++)
        {
            int directionsInt = (int) Direction.CENTER;
            directionsInts.Add(directionsInt | random.Next(upperLimit));
            nodesValues.Add(0);
        }

        int upMask = (int) Direction.UP | (int) Direction.UP_LEFT | (int) Direction.RIGHT_UP;
        int downMask = (int) Direction.LEFT_DOWN | (int) Direction.DOWN | (int) Direction.DOWN_RIGHT;
        int leftMask = (int) Direction.UP_LEFT | (int) Direction.LEFT | (int) Direction.LEFT_DOWN;
        int rightMask = (int) Direction.DOWN_RIGHT | (int) Direction.RIGHT | (int) Direction.RIGHT_UP;
        for(int i=0; i<dimension; i++)
        {
            directionsInts[i] &= ~upMask;
            directionsInts[directionsInts.Count-1-i] &= ~downMask;
            directionsInts[i*dimension] &= ~leftMask;
            directionsInts[(i+1)*dimension-1] &= ~rightMask;
        }

        while(true)
        {
            int clickCount = random.Next(dimension, dimension*dimension);
            for(int i=0; i<clickCount; i++)
            {
                int nodeIndex = random.Next(nodesValues.Count);
                List<(int, int)> neighborsOffsets = DirectionUtility.DirectionIntToOffsets(directionsInts[nodeIndex]);
                foreach((int, int) neighborOffset in neighborsOffsets)
                {
                    int neighborIndex = nodeIndex + neighborOffset.Item1*dimension + neighborOffset.Item2;
                    nodesValues[neighborIndex] = (nodesValues[neighborIndex]+1) % range;
                }
                //Debug.Log(nodeIndex);
            }

            int zeroCount = 0;
            for(int i=0; i<nodesValues.Count; i++)
            {
                if(0 != nodesValues[i])
                {
                    continue;
                }
                zeroCount++;
            }

            if(zeroCount != nodesValues.Count)
            {
                break;
            }
        }
    }    

    public string Base16Encode()
    {
        string res = "";
        res += Convert.ToString(dimension, 16); // 4 bits for dimension
        res += Convert.ToString(range, 16); // 4 bits for range
        res += Convert.ToString(seed, 16).PadLeft(8,'0'); // 32 bits for seed
        for(int i=0; i<dimension*dimension; i++)
        {
            res += Convert.ToString(directionsInts[i], 16).PadLeft(3,'0'); // 12 bits for each directionsInt
            res += Convert.ToString(nodesValues[i], 16); // 4 bits for each nodesValue
        }

        res += Convert.ToString(hasWon, 16); // 4 bits for range
        return res;
    }
    public static LevelInformation Base16Decode(string base16String)
    {
        if("" == base16String)
        {
            return null;
        }

        int startIndex = 0;
        LevelInformation levelInformation = CreateInstance<LevelInformation>();
        levelInformation.dimension = Convert.ToInt32(base16String.Substring(startIndex,1), 16);
        startIndex++;
        levelInformation.range = Convert.ToInt32(base16String.Substring(startIndex,1), 16);
        startIndex++;
        levelInformation.seed = Convert.ToInt32(base16String.Substring(startIndex,8), 16);
        startIndex += 8;

        levelInformation.directionsInts = new List<int>();
        levelInformation.nodesValues = new List<int>();
        for(int i=0; i<levelInformation.dimension*levelInformation.dimension; i++)
        {
            levelInformation.directionsInts.Add(Convert.ToInt32(base16String.Substring(startIndex,3), 16));
            levelInformation.nodesValues.Add(Convert.ToInt32(base16String.Substring(startIndex+3,1), 16));
            startIndex += 4;
        }

        levelInformation.hasWon = Convert.ToInt32(base16String.Substring(startIndex,1), 16);

        return levelInformation;
    }
}

[Serializable]
public class Directions
{
    public List<Direction> directions;
}