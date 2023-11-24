using UnityEngine;
using UnityEngine.Playables;

public class CutsceneHellTrenchFloor8Controller : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;

    public PlayableDirector playableDirector;

    private int index;
    public void Start()
    {
        index = 0;
        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);
    }

    public void ReceiveSignal()
    {
        index++;
        switch(index) 
        {
          case 1:
            playableDirector.Pause();
            DialogueManager.dialogueManager.onDialogueFinish.AddListener(ResumePlayableDirector);
            DialogueManager.dialogueManager.StartDialogue(DialogueName.CUTSCENE_HELL_TRENCH_8_1);
            break;
          case 2:
            playableDirector.Pause();
            DialogueManager.dialogueManager.onDialogueFinish.AddListener(ToNextScene);
            DialogueManager.dialogueManager.StartDialogue(DialogueName.CUTSCENE_HELL_TRENCH_8_2);
            break;
          default:
            break;
        }
    }
    public void ResumePlayableDirector()
    {
        playableDirector.Play();
    }

    public void ToNextScene()
    {
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.HELL_TRENCH_8, 0);
    }

    public void EnableCameraAndUI()
    {
        mainCamera.SetActive(true);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
    }
}
