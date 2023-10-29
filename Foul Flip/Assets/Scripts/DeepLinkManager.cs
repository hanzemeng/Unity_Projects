using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    public static DeepLinkManager deepLinkManager;

    private void Awake()
    {
        if(null != deepLinkManager)
        {
            Destroy(gameObject);
            return;
        }

        deepLinkManager = this;
        DontDestroyOnLoad(gameObject);

        Application.deepLinkActivated += OnDeepLinkActivated;
        if(!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActivated(Application.absoluteURL);
        }

        //OnDeepLinkActivated("flfl://level?3&2&765");
    }

    public void OnDeepLinkActivated(string url)
    {
        int dimension, range, seed;
        string[] parameters = url.Split('?')[1].Split('&');
        if(parameters.Length < 3)
        {
            return;
        }

        if(
            !Int32.TryParse (parameters[0], out dimension) ||
            !Int32.TryParse (parameters[1], out range) ||
            !Int32.TryParse (parameters[2], out seed)
           )
        {
            return;
        }

        LevelInformation newLevelInformation = ScriptableObject.CreateInstance<LevelInformation>();
        newLevelInformation.dimension = dimension;
        newLevelInformation.range = range;
        newLevelInformation.seed = seed;
        newLevelInformation.Randomize();

        string lastLevelInformationSavePath = Path.Combine(Application.persistentDataPath, GameControl.LAST_LEVEL_INFORMATION_SAVE_PATH);
        using (var stream = File.Open(lastLevelInformationSavePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(newLevelInformation.Base16Encode());
            }
        }

        if(null != GameControl.gameControl)
        {
            GameControl.gameControl.LoadLevelFromDisk();
        }
        else
        {
            SettingsInformation currentSettingsInformation = ScriptableObject.CreateInstance<SettingsInformation>();
            currentSettingsInformation.dimension = dimension;
            currentSettingsInformation.range = range;
            currentSettingsInformation.seed = seed;

            string currentSettingsInformationSavePath = Path.Combine(Application.persistentDataPath, SettingsControl.CURRENT_SETTINGS_INFORMATION_SAVE_PATH);
            using (var stream = File.Open(currentSettingsInformationSavePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(currentSettingsInformation.Base16Encode());
                }
            }
            SceneManager.LoadScene("Game");
        }
    }

    public string GenerateDeepLink(int dimension, int range, int seed)
    {
        return $"flfl://level?{dimension}&{range}&{seed}";
    }
}
