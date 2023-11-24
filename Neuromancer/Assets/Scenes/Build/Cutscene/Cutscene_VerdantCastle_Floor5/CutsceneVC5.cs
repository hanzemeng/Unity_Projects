using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneVC5 : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;
    private PlayableDirector director;
    private bool readyToTransition;
    private bool isShowingSkip;
    [SerializeField] private TMP_Text skipText;

    private void Start() {
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);
        director = GetComponent<PlayableDirector>();
        AudioManager.instance.StopBackgroundMusic();
        isShowingSkip = false;
        readyToTransition = false;
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
            director.Pause();
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
        director.Pause();
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_VC5_Pre", false);
    }

    public void StartDialogueAfter() {
        director.Pause();
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_VC5_After", false);
    }

    public void StartDialogueDamarcus() {
        director.Pause();
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumeTimeline);
        DialogueManager.dialogueManager.StartDialogue("Cutscene_VC5_Damarcus", false);
    }

    private void ResumeTimeline() {
        director.Resume();
    }

    public void ToNextScene() {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.VERDANT_CASTLE_5, 0);
    }

    private void EnableCameraAndUI() {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
    }
}
