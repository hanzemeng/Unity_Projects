using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl gameControl;

    private bool isBusy;
    private float timer;

    private const int TWO_BILLION = 2000000000;

    private LevelInformation originalLevelInformation;
    private LevelInformation currentLevelInformation;
    public List<GameObject> nodes;

    private SessionInformation currentSessionInformation;

    public const string LAST_SESSION_INFORMATION_SAVE_PATH = "lastSessionInformation.asset";
    public const string LAST_LEVEL_INFORMATION_SAVE_PATH = "lastLevelInformation.asset";

    private void Awake()
    {
        if(null != gameControl)
        {
            Destroy(gameObject);
            return;
        }
        gameControl = this;
        DontDestroyOnLoad(gameObject);
        timer = 0f;
    }

    private void Start()
    {
        PostProcessing.postProcessing.StartBlink();
        GameUI.gameUI.AnimateSwipeText();
        StartListenInputs();

        string lastSessionInformationSavePath = Path.Combine(Application.persistentDataPath, LAST_SESSION_INFORMATION_SAVE_PATH);
        string lastSessionInformationString = "";
        if(File.Exists(lastSessionInformationSavePath))
        {
            using(var stream = File.Open(lastSessionInformationSavePath, FileMode.Open))
            {
                using(var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    lastSessionInformationString = reader.ReadString();
                }
            }
        }
        currentSessionInformation = SessionInformation.Base16Decode(lastSessionInformationString);
        if(null == currentSessionInformation)
        {
            currentSessionInformation = ScriptableObject.CreateInstance<SessionInformation>();
        }

        GameUI.gameUI.ChangeStreakText(currentSessionInformation.winStreak, new Color(1f,1f,1f,1f));

        string lastLevelInformationSavePath = Path.Combine(Application.persistentDataPath, LAST_LEVEL_INFORMATION_SAVE_PATH);
        string lastLevelInformationString = "";
        if(File.Exists(lastLevelInformationSavePath))
        {
            using (var stream = File.Open(lastLevelInformationSavePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    lastLevelInformationString = reader.ReadString();
                }
            }
        }
        originalLevelInformation = LevelInformation.Base16Decode(lastLevelInformationString);
        if(null == originalLevelInformation)
        {
            SettingsInformation settingsInformation = SettingsControl.settingsControl.GetCurrentSettings();
            originalLevelInformation = ScriptableObject.CreateInstance<LevelInformation>();
            originalLevelInformation.dimension = settingsInformation.dimension;
            originalLevelInformation.range = settingsInformation.range;
            originalLevelInformation.seed = settingsInformation.seed;
            CreateNewLevel();
        }
        else
        {
            ResetCurrentLevel();
        }
    }

    private void FixedUpdate()
    {
        if(isBusy)
        {
            return;
        }

        if(0 == currentLevelInformation.hasWon)
        {
            PostProcessing.postProcessing.AddBlinkIntensityBy(2f*Time.deltaTime);
            timer += Time.deltaTime;
            if(currentSessionInformation.elapsedSecond != (int)timer)
            {
                currentSessionInformation.elapsedSecond = (int)timer;
                GameUI.gameUI.ChangeTimeText(currentSessionInformation.elapsedSecond, new Color(1f,0f,0f,1f));
            }
        }
    }

    public void LoadLevelFromDisk()
    {
        if(SettingsControl.settingsControl.IsOpen())
        {
            SettingsControl.settingsControl.OnDiscardChangeClick();
        }
        if(AlertControl.alertControl.IsOpen())
        {
            AlertControl.alertControl.OnReturnGameClick();
        }

        string lastLevelInformationSavePath = Path.Combine(Application.persistentDataPath, LAST_LEVEL_INFORMATION_SAVE_PATH);
        string lastLevelInformationString = "";
        if(File.Exists(lastLevelInformationSavePath))
        {
            using (var stream = File.Open(lastLevelInformationSavePath, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    lastLevelInformationString = reader.ReadString();
                }
            }
        }
        originalLevelInformation = LevelInformation.Base16Decode(lastLevelInformationString);
        SettingsInformation settingsInformation = SettingsControl.settingsControl.GetCurrentSettings();
        if(null == originalLevelInformation)
        {
            originalLevelInformation = ScriptableObject.CreateInstance<LevelInformation>();
            originalLevelInformation.dimension = settingsInformation.dimension;
            originalLevelInformation.range = settingsInformation.range;
            originalLevelInformation.seed = settingsInformation.seed;
            CreateNewLevel();
        }
        else
        {
            settingsInformation.dimension = originalLevelInformation.dimension;
            settingsInformation.range = originalLevelInformation.range;
            settingsInformation.seed = originalLevelInformation.seed;
            ResetCurrentLevel();
        }
    }

    private void StartListenInputs()
    {
        GameUserInput.gameUserInput.ListenNodesInput();
        GameUserInput.gameUserInput.ListenNewLevelSlider();
        GameUserInput.gameUserInput.ListenSettingsInput();
        GameUserInput.gameUserInput.ListenAlertInput();
        GameUserInput.gameUserInput.ListenFacebookInput();
        GameUserInput.gameUserInput.ListenTwitterInput();
    }
    private void StopListenInputs()
    {
        GameUserInput.gameUserInput.StopListenNodesInput();
        GameUserInput.gameUserInput.StopListenNewLevelSlider();
        GameUserInput.gameUserInput.StopListenSettingsInput();
        GameUserInput.gameUserInput.StopListenAlertInput();
        GameUserInput.gameUserInput.StopListenFacebookInput();
        GameUserInput.gameUserInput.StopListenTwitterInput();
    }

    private void OnApplicationQuit()
    {
        string lastSessionInformationSavePath = Path.Combine(Application.persistentDataPath, LAST_SESSION_INFORMATION_SAVE_PATH);
        using (var stream = File.Open(lastSessionInformationSavePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(currentSessionInformation.Base16Encode());
            }
        }

        string lastLevelInformationSavePath = Path.Combine(Application.persistentDataPath, LAST_LEVEL_INFORMATION_SAVE_PATH);
        using (var stream = File.Open(lastLevelInformationSavePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(originalLevelInformation.Base16Encode());
            }
        }
    }

    public void OnSettingsClick()
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;
        AudioManager.audioManager.PlayIconClick();
        PostProcessing.postProcessing.AddBlinkIntensityBy(-100f);
        StopListenInputs();
        SettingsControl.settingsControl.OpenSettings();
    }
    public void OnSettingsClose(bool appliesChanges)
    {
        StartListenInputs();

        if(appliesChanges)
        {
            SettingsInformation settingsInformation = SettingsControl.settingsControl.GetCurrentSettings();
            originalLevelInformation.dimension = settingsInformation.dimension;
            originalLevelInformation.range = settingsInformation.range;
            originalLevelInformation.seed = settingsInformation.seed;
            CreateNewLevel();
        }

        isBusy = false;
    }

    public void OnAlertClick()
    {
        if(isBusy)
        {
            return;
        }

        isBusy = true;
        AudioManager.audioManager.PlayIconClick();
        PostProcessing.postProcessing.AddBlinkIntensityBy(-100f);
        StopListenInputs();
        AlertControl.alertControl.OpenAlert();
        
    }
    public void OnAlertClose()
    {
        StartListenInputs();
        isBusy = false;
    }

    public void OnNodeClick(int nodeIndex)
    {
        if(isBusy)
        {
            return;
        }

        AudioManager.audioManager.PlayNodeClick();

        PostProcessing.postProcessing.AddBlinkIntensityBy(-10f);

        List<(int, int)> neighborsOffsets = DirectionUtility.DirectionIntToOffsets(currentLevelInformation.directionsInts[nodeIndex]);
        foreach((int, int) neighborOffset in neighborsOffsets)
        {
            int neighborIndex = nodeIndex + neighborOffset.Item1*currentLevelInformation.dimension + neighborOffset.Item2;
            currentLevelInformation.nodesValues[neighborIndex] = (currentLevelInformation.nodesValues[neighborIndex]+1) % currentLevelInformation.range;
            GameUI.gameUI.ChangeNodeColorTo(neighborIndex, currentLevelInformation.nodesValues[neighborIndex]);
            GameUI.gameUI.ShootArrow(nodeIndex, neighborIndex);
        }

        if(1 == currentLevelInformation.hasWon)
        {
            return;
        }

        currentSessionInformation.flip += 1;
        GameUI.gameUI.ChangeFlipText(currentSessionInformation.flip, new Color(1f,0f,0f,1f));

        bool hasWon = true;
        foreach(int nodeValue in currentLevelInformation.nodesValues)
        {
            if(0 != nodeValue)
            {
                hasWon = false;
                break;
            }
        }
        if(hasWon)
        {
            AudioManager.audioManager.PlayWin();
            currentLevelInformation.hasWon = 1;
            currentSessionInformation.winStreak += 1;
            PostProcessing.postProcessing.AddBlinkIntensityBy(-100f);
            GameUI.gameUI.ChangeStreakText(currentSessionInformation.winStreak, new Color(0f,1f,0f,1f));
            GameUI.gameUI.StartFlickerFacebookIcon();
            GameUI.gameUI.StartFlickerTwitterIcon();
            GameUI.gameUI.FadeInSolvedCanvasGroup();
        }
    }

    public void OnSwipeLeft() // reset level
    {
        if(isBusy)
        {
            return;
        }

        if(!InAppPurchaseManager.inAppPurchaseManager.HasRemoveAds())
        {
            if(!AdsManager.adsManager.AdIsReady() || Random.value > 0.2f)
            {
                AdsManager.adsManager.LoadAd();
            }
            else
            {
                AdsManager.adsManager.ShowAd();
            }
        }
        
        AudioManager.audioManager.PlaySwipe();
        PostProcessing.postProcessing.AddBlinkIntensityBy(-50f);
        ResetCurrentLevel();
    }
    public void OnSwipeRight() // new level
    {
        if(isBusy)
        {
            return;
        }

        if(!InAppPurchaseManager.inAppPurchaseManager.HasRemoveAds())
        {
            if(!AdsManager.adsManager.AdIsReady() || Random.value > 0.4f)
            {
                AdsManager.adsManager.LoadAd();
            }
            else
            {
                AdsManager.adsManager.ShowAd();
            }
        }

        AudioManager.audioManager.PlaySwipe();
        PostProcessing.postProcessing.AddBlinkIntensityBy(-50f);
        originalLevelInformation.seed = (originalLevelInformation.seed + 1) % TWO_BILLION;
        CreateNewLevel();
    }

    private void ResetCurrentLevel()
    {
        StartCoroutine(ResetCurrentLevelCoroutine());
    }
    private IEnumerator ResetCurrentLevelCoroutine()
    {
        isBusy = true;

        yield return new WaitForSeconds(GameUI.gameUI.FadeOutNodesCanvasGroup());

        if(null != currentLevelInformation && 1 == currentLevelInformation.hasWon)
        {
            currentSessionInformation.winStreak -= 1;
            GameUI.gameUI.ChangeStreakText(currentSessionInformation.winStreak, new Color(1f,0f,0f,1f));
            GameUI.gameUI.FadeOutSolvedCanvasGroup();
        }
        currentLevelInformation = originalLevelInformation.DeepCopy();
        
        nodes = GameUI.gameUI.CreateNodes(currentLevelInformation);
        GameUserInput.gameUserInput.ListenNodesInput();

        currentSessionInformation.flip = 0;
        GameUI.gameUI.ChangeFlipText(currentSessionInformation.flip, new Color(0f,1f,0f,1f));
        timer = 0f;
        currentSessionInformation.elapsedSecond = (int)timer;
        GameUI.gameUI.ChangeTimeText(currentSessionInformation.elapsedSecond, new Color(0f,1f,0f,1f));
        GameUI.gameUI.ChangeSeedText(currentLevelInformation.seed);
        GameUI.gameUI.StopFlickerFacebookIcon();
        GameUI.gameUI.StopFlickerTwitterIcon();

        yield return new WaitForSeconds(0.25f);
        yield return new WaitForSeconds(GameUI.gameUI.FadeInNodesCanvasGroup());

        isBusy = false;
    }

    private void CreateNewLevel()
    {
        StartCoroutine(CreateNewLevelCoroutine());
    }
    private IEnumerator CreateNewLevelCoroutine()
    {
        isBusy = true;

        yield return new WaitForSeconds(GameUI.gameUI.FadeOutNodesCanvasGroup());

        if(null != currentLevelInformation && 0 == currentLevelInformation.hasWon)
        {
            currentSessionInformation.winStreak = 0;
            GameUI.gameUI.ChangeStreakText(currentSessionInformation.winStreak, new Color(1f,0f,0f,1f));
        }
        else if(null != currentLevelInformation && 1 == currentLevelInformation.hasWon)
        {
            GameUI.gameUI.FadeOutSolvedCanvasGroup();
        }

        originalLevelInformation.Randomize();
        currentLevelInformation = originalLevelInformation.DeepCopy();
        nodes = GameUI.gameUI.CreateNodes(currentLevelInformation);
        GameUserInput.gameUserInput.ListenNodesInput();

        currentSessionInformation.flip = 0;
        GameUI.gameUI.ChangeFlipText(currentSessionInformation.flip, new Color(0f,1f,0f,1f));
        timer = 0f;
        currentSessionInformation.elapsedSecond = (int)timer;
        GameUI.gameUI.ChangeTimeText(currentSessionInformation.elapsedSecond, new Color(0f,1f,0f,1f));
        GameUI.gameUI.ChangeSeedText(currentLevelInformation.seed);
        GameUI.gameUI.StopFlickerFacebookIcon();
        GameUI.gameUI.StopFlickerTwitterIcon();

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(GameUI.gameUI.FadeInNodesCanvasGroup());

        isBusy = false;
    }

    // https://apps.apple.com/us/app/foulflip/id6467679071
    public void OnFacebookClick()
    {
        if(isBusy)
        {
            return;
        }

        string url = $"https://djangoserver.hanmingjie.com/foulflip?dimension={originalLevelInformation.dimension}%26range={originalLevelInformation.range}%26seed={originalLevelInformation.seed}";
        
        Application.OpenURL("https://www.facebook.com/dialog/feed?" +
                            "app_id=1476191406507325" +
                            "&display=popup" +
                            $"&link={url}" +
                            $"&redirect_uri={url}"
                            );
    }

    public void OnTwitterClick()
    {
        if(isBusy)
        {
            return;
        }

        string url = $"https://djangoserver.hanmingjie.com/foulflip?dimension={originalLevelInformation.dimension}%26range={originalLevelInformation.range}%26seed={originalLevelInformation.seed}";

        Application.OpenURL("http://twitter.com/intent/tweet?" +
                            "text=Can%20you%20solve%20this%20puzzle%3F" +
                            "&hashtags=FoulFlip" +
                            $"&url={url}"
                            );
        
    }
}
