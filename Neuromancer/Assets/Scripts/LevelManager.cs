using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class LevelName
{
    public const string INITIALIZATION = "Initialization";

    public const string TITLE = "Title";
    public const string TOWN_START = "Town_start";
    public const string OW_Start = "OW_start_reworked";
    public const string TOWN_START_DESTROYED = "Town_start_destroyed";
    public const string OW_Start_Progress = "OW_start_progress";
    public const string OW_BLACK_FOREST = "OW_1_BlackForest";
    public const string OW_START_CAVE = "Cave_to_Blacksmith";
    public const string FIELD_START = "Field_Start";
    public const string FIELD_DESTROYED = "Field_Destroyed";
    public const string CATACOMB_1 = "Catacomb_Floor1";
    public const string CATACOMB_2 = "Catacomb_Floor2";
    public const string CATACOMB_3 = "Catacomb_Floor3";
    public const string CATACOMB_4 = "Catacomb_Floor4";
    public const string SWAMP_ENTER = "Swamp_Entrance";
    public const string SWAMP_1 = "Swamp_Floor1";
    public const string SWAMP_2 = "Swamp_Floor2";
    public const string SWAMP_3 = "Swamp_Floor3";
    public const string SWAMP_4 = "Swamp_Floor4";
    public const string VERDANT_CASTLE_1 = "VerdantCastle_Floor1";
    public const string VERDANT_CASTLE_2 = "VerdantCastle_Floor2";
    public const string VERDANT_CASTLE_3 = "VerdantCastle_Floor3";
    public const string VERDANT_CASTLE_4 = "VerdantCastle_Floor4";
    public const string VERDANT_CASTLE_5 = "VerdantCastle_Floor5";
    public const string HELL_TRENCH_1 = "HellTrench_Floor1";
    public const string HELL_TRENCH_2 = "HellTrench_Floor2";
    public const string HELL_TRENCH_3 = "HellTrench_Floor3";
    public const string HELL_TRENCH_4 = "HellTrench_Floor4";
    public const string HELL_TRENCH_5 = "HellTrench_Floor5";
    public const string HELL_TRENCH_6 = "HellTrench_Floor6";
    public const string HELL_TRENCH_7 = "HellTrench_Floor7";
    public const string HELL_TRENCH_8 = "HellTrench_Floor8";
    public const string HELL = "Hell";

    public const string CUTSCENE_INTRO = "Cutscene_Introduction";
    public const string CUTSCENE_TOWN_START_1 = "Cutscene_Town_Start_1";
    public const string CUTSCENE_TOWN_START_2 = "Cutscene_Town_Start_2";
    public const string CUTSCENE_VERDANT_CASTLE_5 = "Cutscene_VC5";
    public const string CUTSCENE_HELL_1 = "Cutscene_Hell_1";
    public const string CUTSCENE_HELL_2 = "Cutscene_Hell_2";
    public const string CUTSCENE_EPILOGUE = "Cutscene_Epilogue";
    public const string CUTSCENE_HELL_TRENCH_8 = "Cutscene_HellTrench_Floor8";
    public const string CUTSCENE_CREDITS = "Cutscene_Credits";
    public const string UNIT_TEST = "Zach_UnitTest";

}

/*
    Usage:
        LoadLevel(levelName) to transition to levelName.
*/

public class LevelManager : MonoBehaviour
{
    public static LevelManager levelManager;
    private IEnumerator currentCoroutine;

    [SerializeField] private Image loadingBackground;
    [SerializeField] private Image loadingImage;
    private TweenerColor loadingBackgroundTweener;
    private TweenerColor loadingImageTweener;

    public string lastLevelName;
    public int lastSpawnPointIndex;
    
    [HideInInspector] public UnityEvent onCurrentSceneBlack = new UnityEvent();
    [HideInInspector] public UnityEvent onNewSceneEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<string> onNewSceneLoadAsyncEvent = new UnityEvent<string>();     // For DeveloperConsole such that any allies the player has in the party have their navmeshAgent disabled.

    private void Awake()
    {
        if (null == levelManager)
        {
            levelManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        loadingBackgroundTweener = new TweenerColor(this, res => loadingBackground.color = res);
        loadingBackground.rectTransform.sizeDelta = new Vector2(ScreenManager.GAME_WINDOW_WIDTH, ScreenManager.GAME_WINDOW_HEIGHT); // loading image's size is the same as game window size
        loadingImageTweener = new TweenerColor(this, res => loadingImage.color = res);
        loadingImage.rectTransform.sizeDelta = new Vector2(ScreenManager.GAME_WINDOW_WIDTH, ScreenManager.GAME_WINDOW_HEIGHT);
    }
    private void Start()
    {
        LoadLevel(LevelName.TITLE, 0);
    }

    public void ReloadCurrent()
    {
        LoadLevel(lastLevelName, lastSpawnPointIndex);
    }

    public void LoadLevel(string levelName, int spawnPointIndex)
    {
        if (null == currentCoroutine)
        {
            currentCoroutine = LoadLevelCorutine(levelName, spawnPointIndex);
            StartCoroutine(currentCoroutine);
        }
    }

    private IEnumerator LoadLevelCorutine(string levelName, int spawnPointIndex)
    {
        onNewSceneLoadAsyncEvent.Invoke(levelName);
        loadingBackgroundTweener.TweenWithTime(loadingBackground.color, new Color(1f, 1f, 1f, 1f), 0.4f, Tweener.LINEAR);
        while (loadingBackgroundTweener.IsTweening())
        {
            yield return null;
        }

        onCurrentSceneBlack.Invoke();
        onCurrentSceneBlack.RemoveAllListeners();

        PlayerInputManager.playerInputs.Disable();
        SaveLoadManager.saveLoadManager.SaveUnitData();
        
        float elapsedTime = 0;
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(levelName);
        while (!loadingScene.isDone)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime>0.5f && !loadingImageTweener.IsTweening())
            {
                loadingImageTweener.TweenWithTime(loadingImage.color, new Color(1f, 1f, 1f, 1f), 1.0f, Tweener.LINEAR);
            }
            yield return null;
        }
        PlayerController.player.enabled = false;
        CameraController.current.virtualCam.enabled = false;
        SaveLoadManager.saveLoadManager.LoadUnitData(spawnPointIndex);
        InitializeObjects();
        yield return null;
        PlayerController.player.enabled = true;
        CameraController.current.virtualCam.enabled = true;

        loadingBackgroundTweener.TweenWithTime(loadingBackground.color, new Color(1f, 1f, 1f, 0f), 1.5f, Tweener.LINEAR);
        loadingImageTweener.TweenWithTime(loadingImage.color, new Color(1f, 1f, 1f, 0f), 1.5f, Tweener.LINEAR);
        currentCoroutine = null;
        PlayerInputManager.playerInputs.Enable();
        onNewSceneEvent.Invoke();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Period))
        {
            ReloadCurrent();
        }
    }

    private void InitializeObjects() {
        List<ObjectData> objects = SaveLoadManager.saveLoadManager.allObjects;
        ObjectPermanent[] permanentObjects = FindObjectsByType<ObjectPermanent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<ObjectPermanent> permanents = new List<ObjectPermanent>(permanentObjects);
        foreach (ObjectData od in objects) {
            GameObject obj = permanents.Find(item => item.GetComponent<CleverCrow.Fluid.UniqueIds.UniqueId>().Id == od.id)?.gameObject;
            if (obj) {
                if (od.state == 0) { obj.SetActive(true); }
                else if (od.state == 1) { obj.SetActive(false); }
                else if (od.state == 2) { Destroy(obj); }
            }
        }
    }
}