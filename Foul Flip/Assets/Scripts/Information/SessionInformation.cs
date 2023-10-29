using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SESSION_INFORMATION", menuName = "ScriptableObjects/Session Information", order = 1)]
public class SessionInformation : ScriptableObject
{
    public int flip;
    public int elapsedSecond;
    public int winStreak;

    private SessionInformation()
    {
        flip = 0;
        elapsedSecond = 0;
        winStreak = 0;
    }

    public SessionInformation DeepCopy()
    {
        SessionInformation newSessionInformation = CreateInstance<SessionInformation>();

        newSessionInformation.flip = flip;
        newSessionInformation.elapsedSecond = elapsedSecond;
        newSessionInformation.winStreak = winStreak;

        return newSessionInformation;
    }

    public string Base16Encode()
    {
        string res = "";
        res += Convert.ToString(flip, 16).PadLeft(8, '0'); // 32 bits for flip
        res += Convert.ToString(elapsedSecond, 16).PadLeft(8, '0'); // 32 bits for elapsedSecond
        res += Convert.ToString(winStreak, 16).PadLeft(8, '0'); // 32 bits for winStreak
        return res;
    }
    public static SessionInformation Base16Decode(string base16String)
    {
        if("" == base16String)
        {
            return null;
        }

        int startIndex = 0;
        SessionInformation sessionInformation = CreateInstance<SessionInformation>();

        sessionInformation.flip = Convert.ToInt32(base16String.Substring(startIndex,8), 16);
        startIndex += 8;
        sessionInformation.elapsedSecond = Convert.ToInt32(base16String.Substring(startIndex,8), 16);
        startIndex += 8;
        sessionInformation.winStreak = Convert.ToInt32(base16String.Substring(startIndex,8), 16);

        return sessionInformation;
    }
}
