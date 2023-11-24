using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Cutscene_Town_Start_1 : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;
    private GameObject mapManager;
    private PlayableDirector director;
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private TMP_Text skipText;
    private bool readyToTransition;
    private bool isShowingSkip;

    private void Start() {
        AudioManager.instance.PlayBackgroundMusic(AudioManager.SoundResource.BGM_VILLIAGE);
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mapManager = GameObject.Find("Map Managers");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);
        mapManager.SetActive(false);
        director = GetComponent<PlayableDirector>();
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

    public void StartDialoguePre() {
        //director.Pause();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_Town_Start_1_Pre");
    }

    public void StartDialogueWarning() {
        //director.Pause();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_Town_Start_1_Warning");
    }

    public void StartDialogueStart() {
        //director.Pause();
        director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_Town_Start_1_Start");
    }

    private void ResumeTimeline() {
        //director.Resume();
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
    }

    public void ToNextScene() {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.TOWN_START, 0);
    }

    private void EnableCameraAndUI() {
        cutsceneCamera.SetActive(false);
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
        mapManager.SetActive(true);
    }
}
