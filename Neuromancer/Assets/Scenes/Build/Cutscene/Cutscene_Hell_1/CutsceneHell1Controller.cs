using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneHell1Controller : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;

    private bool isShowingSkip;
    private bool readyToTransition;
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private PlayableDirector playableDirector;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);

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
            playableDirector.Pause();
            DialogueManager.dialogueManager.EndDialogue();
        }
    }

    private IEnumerator ShowSkipTextCoroutine()
    {
        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += 0.8f * Time.deltaTime;
            skipText.color = Color.Lerp(new Color(1f,1f,1f,0f), Color.white, lerpAmount);
            yield return null;
        }
        skipText.color = Color.white;
        readyToTransition = true;
    }

    public void ReceiveSignal()
    {
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ToNextScene);
        DialogueManager.dialogueManager.StartDialogue(DialogueName.CUTSCENE_HELL_1, false);
    }

    public void ToNextScene()
    {
        readyToTransition = false;
        
        LevelManager.levelManager.lastLevelName = LevelName.HELL;
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.HELL, 0);
    }

    public void EnableCameraAndUI()
    {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
    }
}
