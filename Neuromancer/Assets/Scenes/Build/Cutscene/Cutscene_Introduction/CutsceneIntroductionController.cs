using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using System;

public class CutsceneIntroductionController : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject virtualCamera;
    private GameObject UICanvases;
    private GameObject dialogueNextButton;
    private PlayableDirector director;
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private string[] allDialogueFilenames;
    [SerializeField] private Material hellRealmSkyboxMaterial;
    [SerializeField] private Material normalSkyBoxMaterial;
    private Material currentSkyboxMaterial;
    private bool hasSkyboxChanged = false;
    private bool readyToTransition;
    private bool isShowingSkip;
    private int currentIndex = 0;
    private bool isExitingScene = false;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        director = GetComponent<PlayableDirector>();

        mainCamera = GameObject.Find("Main Camera");
        virtualCamera = GameObject.Find("VirtualCam");
        UICanvases = GameObject.Find("UI Canvases");
        dialogueNextButton = GameObject.Find("NextButton");
        if(dialogueNextButton != null)
        {
            dialogueNextButton.SetActive(false);
        }

        mainCamera.SetActive(false);
        virtualCamera.SetActive(false);
        UICanvases.SetActive(false);

        PlayerInputManager.playerInputs.PlayerAction.Disable(); 
        PlayerInputManager.playerInputs.CameraAction.Disable(); 
        PlayerInputManager.playerInputs.AllyAction.Disable(); 
        PlayerInputManager.playerInputs.DialogueAction.Disable(); 

        //AudioManager.instance.PauseBackgroundMusic(AudioManager.SoundResource.BGM_VILLIAGE);
        //AudioManager.instance.StopBackgroundMusic();
        //StartCoroutine(BeginExposition(delayBeforeDialogueStart));
    }

    private void Update()
    {
        // my dumb solution to keeping the player inputs off because doing it in Start() isn't working for some reason. 
        if(!isExitingScene)
        {
            PlayerInputManager.playerInputs.PlayerAction.Disable(); 
            PlayerInputManager.playerInputs.CameraAction.Disable(); 
            PlayerInputManager.playerInputs.AllyAction.Disable(); 
            PlayerInputManager.playerInputs.DialogueAction.Disable(); 
        }


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
            lerpAmount += 0.8f * Time.deltaTime;
            skipText.color = Color.Lerp(new Color(1f,1f,1f,0f), Color.white, lerpAmount);
            yield return null;
        }
        skipText.color = Color.white;
        readyToTransition = true;
    }

    public void ChangeSkybox()
    {
        if(!hasSkyboxChanged)
        {
            RenderSettings.skybox = hellRealmSkyboxMaterial;
            hasSkyboxChanged = true;
        }
        else
        {
            RenderSettings.skybox = normalSkyBoxMaterial;
            hasSkyboxChanged = false;
        }
    }

    public void StartDialogue()
    {
        if(currentIndex < allDialogueFilenames.Length) {    DialogueManager.dialogueManager.StartDialogue(allDialogueFilenames[currentIndex], pauseGame: false); }
        currentIndex += 1;
        //DialogueManager.dialogueManager.StartDialogue("Cutscene_Introduction", pauseGame: false);
    }

    public void GoToNextDialogueLine()
    {
        DialogueManager.dialogueManager.UpdateTextBox();
    }
    public void ToNextScene()
    {
        isExitingScene = true;
        DialogueManager.dialogueManager.onDialogueFinish.RemoveAllListeners();
        DialogueManager.dialogueManager.EndDialogue();
        LevelManager.levelManager.onCurrentSceneBlack.AddListener(EnableCameraAndUI);
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_TOWN_START_1, 0);
    }

    private IEnumerator DelayBeforeSceneTransition(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToNextScene();
    }

    public void EnableCameraAndUI()
    {
        mainCamera.SetActive(true);
        cutsceneCamera.SetActive(false);
        virtualCamera.SetActive(true);
        UICanvases.SetActive(true);
        if(dialogueNextButton != null)
        {
            dialogueNextButton.SetActive(true);
        }
        PlayerInputManager.playerInputs.PlayerAction.Enable(); 
        PlayerInputManager.playerInputs.CameraAction.Enable(); 
        PlayerInputManager.playerInputs.AllyAction.Enable(); 
        PlayerInputManager.playerInputs.DialogueAction.Enable(); 
    }

}
