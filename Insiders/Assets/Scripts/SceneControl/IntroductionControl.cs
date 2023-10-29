using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroductionControl : MonoBehaviour
{   
    public GameObject mainCamera;
    int startFrame;

    public Toggle enableEasyMode;
    public Toggle showTutorial;
    public Toggle showKeyObjectOutline;
    public Toggle isWindowed;
    public GameObject isWindowedObject;

    public AudioSource gameStartSound;
    public AudioSource gameCreditSound;
    public AudioSource gameQuitSound;
    public AudioSource whisperSound;
    IEnumerator whisperRoutine;

    public Button gameStartButton;
    public Button creditsButton;
    public Button gameQuitButton;

    void Start()
    {
        if(Screen.currentResolution.height<1080 || Screen.currentResolution.width<1920)
        {
            isWindowedObject.SetActive(false);
        }
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        startFrame = Time.frameCount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Screen.fullScreen = true;
        StartCoroutine(ShowIntroduction());
        whisperRoutine = StartWhisper();
        StartCoroutine(whisperRoutine);
    }
    IEnumerator ShowIntroduction()
    {
        GlobalVariable.TAKING_INPUT = false;
        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(2f);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GlobalVariable.TAKING_INPUT = true;
    }
    IEnumerator StartWhisper()
    {
        whisperSound.Play();
        float lerpAmount = 0f;
        while(lerpAmount < 0.3f)
        {
            lerpAmount += 0.006f*Time.deltaTime;
            whisperSound.volume = lerpAmount;
            yield return null;
        }
    }
    IEnumerator StopWhisper()
    {
        float lerpAmount = whisperSound.volume;
        while(lerpAmount > 0f)
        {
            lerpAmount -= 0.5f*Time.deltaTime;
            whisperSound.volume = lerpAmount;
            yield return null;
        }
    }

    void Update()
    {
        if(Time.frameCount-startFrame > 2)
        {
            mainCamera.SetActive(true);
        }
    }

    public void ToggleScreenSize()
    {
        Screen.fullScreen = !isWindowed.isOn;
    }

    public void ToGame()
    {
        if(GlobalVariable.TAKING_INPUT)
        {
            StartCoroutine(C_ToGame());
        }

    }
    IEnumerator C_ToGame()
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameSettings.isInEasyMode = enableEasyMode.isOn;
        GameSettings.showTutorial = showTutorial.isOn;
        GameSettings.showObjectOutline = showKeyObjectOutline.isOn;
        
        
        gameStartSound.Play();
        StopCoroutine(whisperRoutine);
        whisperRoutine = StopWhisper();
        StartCoroutine(whisperRoutine);
        ColorBlock temp = gameStartButton.colors;
        temp.normalColor = new Color(1f,0f,0f,1f);
        gameStartButton.colors = temp;

        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Game");
    }

    public void ToCredits()
    {
        if(GlobalVariable.TAKING_INPUT)
        {
            StartCoroutine(C_ToCredits());
        }

    }
    IEnumerator C_ToCredits()
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        gameCreditSound.Play();
        StopCoroutine(whisperRoutine);
        whisperRoutine = StopWhisper();
        StartCoroutine(whisperRoutine);
        ColorBlock temp = creditsButton.colors;
        temp.normalColor = new Color(1f,0f,0f,1f);
        creditsButton.colors = temp;

        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        if(GlobalVariable.TAKING_INPUT)
        {
            StartCoroutine(C_QuitGame());
        }
    }
    IEnumerator C_QuitGame()
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameQuitSound.Play();
        StopCoroutine(whisperRoutine);
        whisperRoutine = StopWhisper();
        StartCoroutine(whisperRoutine);
        ColorBlock temp = gameQuitButton.colors;
        temp.normalColor = new Color(1f,0f,0f,1f);
        gameQuitButton.colors = temp;

        yield return new WaitForSeconds(1f);
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}
