using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneHell2Controller : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;

    private bool isShowingSkip;
    private bool readyToTransition;
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private PlayableDirector playableDirector;

    public void Start()
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
            readyToTransition = false;
            playableDirector.Pause();
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

    public void ReceiveSignal()
    {
        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ToNextScene);
        DialogueManager.dialogueManager.StartDialogue(DialogueName.CUTSCENE_HELL_2, false);
    }

    public void ToNextScene()
    {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_EPILOGUE, 0);
    }

    public void EnableCameraAndUI()
    {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
    }
}
