using UnityEngine;

public class CutsceneEpilogueController : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;

    public void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);

        DialogueManager.dialogueManager.onDialogueFinish.AddListener(ToNextScene);
        DialogueManager.dialogueManager.StartDialogue(DialogueName.CUTSCENE_EPILOGUE, false);
    }

    public void ToNextScene()
    {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_CREDITS, 0);
    }

    public void EnableCameraAndUI()
    {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
    }
}
