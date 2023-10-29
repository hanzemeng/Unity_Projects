using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SETTINGS_INFORMATION", menuName = "ScriptableObjects/Settings Information", order = 2)]
public class SettingsInformation : ScriptableObject
{
    public int dimension;
    public int range;
    public int seed;


    public SettingsInformation DeepCopy()
    {
        SettingsInformation newSettingsInformation = CreateInstance<SettingsInformation>();

        newSettingsInformation.dimension = dimension;
        newSettingsInformation.range = range;
        newSettingsInformation.seed = seed;

        return newSettingsInformation;
    }

    public string Base16Encode()
    {
        string res = "";
        res += Convert.ToString(dimension, 16); // 4 bits for dimension
        res += Convert.ToString(range, 16); // 4 bits for range
        res += Convert.ToString(seed, 16).PadLeft(8, '0'); // 32 bits for seed
        return res;
    }
    public static SettingsInformation Base16Decode(string base16String)
    {
        if("" == base16String)
        {
            return null;
        }

        int startIndex = 0;
        SettingsInformation settingsInformation = CreateInstance<SettingsInformation>();

        settingsInformation.dimension = Convert.ToInt32(base16String.Substring(startIndex,1), 16);
        startIndex += 1;
        settingsInformation.range = Convert.ToInt32(base16String.Substring(startIndex,1), 16);
        startIndex += 1;
        settingsInformation.seed = Convert.ToInt32(base16String.Substring(startIndex,8), 16);

        return settingsInformation;
    }
}
