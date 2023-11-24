using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Cutscene_Town_Start_2 : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;
    private GameObject mapManager;
    private PlayableDirector director;
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private Light sceneSun;
    [SerializeField] private Color targetSunColor;
    [SerializeField] private float sunColorChangeDuration = 0.5f;
    
    private TweenerColor currentLightTweener;

    [SerializeField] private TMP_Text skipText;
    private bool readyToTransition;
    private bool isShowingSkip;

    private void Start() {
        // Autosave to spawn in town_start_destroyed
        if(LevelManager.levelManager != null) {
            LevelManager.levelManager.lastLevelName = "Town_start_destroyed";
            LevelManager.levelManager.lastSpawnPointIndex = 0;
        }
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mapManager = GameObject.Find("Map Managers");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);
        mapManager.SetActive(false);
        director = GetComponent<PlayableDirector>();
        currentLightTweener = new TweenerColor(this, res=>sceneSun.color=res);
        //AudioManager.instance.StopBackgroundMusic();
    }

    private void Update()
    {
        if(Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !isShowingSkip)
        {
            isShowingSkip = true;
            StartCoroutine(ShowSkipTextCoroutine());
        }
        if(Input.GetKeyDown(KeyCode.Z) && readyToTransition)
        {
            readyToTransition = false;
            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            DialogueManager.dialogueManager.onDialogueFinish.RemoveAllListeners();
            DialogueManager.dialogueManager.EndDialogue();
            ToNextScene();
        }
    }

    private IEnumerator ShowSkipTextCoroutine()
    {
        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += 0.58f * Time.deltaTime;
            skipText.color = Color.Lerp(new Color(1f,1f,1f,0f), Color.white, lerpAmount);
            yield return null;
        }
        skipText.color = Color.white;
        readyToTransition = true;
    }

    public void ChangeLightColor()
    {
        AudioManager.instance.StopBackgroundMusic();
        currentLightTweener.TweenWithTime(sceneSun.color, targetSunColor, sunColorChangeDuration, Tweener.LINEAR);
    }

    public void StartDialoguePre() 
    {
        //director.Pause();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_Town_Start_2_Pre");
    }

    public void StartDialogueEnd() 
    {
        //director.Pause();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_Town_Start_2_End");
        AudioManager.instance.PlayBackgroundMusic(AudioManager.SoundResource.BGM_BATTLE1);
    }

    private void ResumeTimeline() 
    {
        //director.Resume();
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    public void ToNextScene() {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.TOWN_START_DESTROYED, 0);
    }

    private void EnableCameraAndUI() {
        cutsceneCamera.SetActive(false);
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
        mapManager.SetActive(true);
    }
}
