using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsControl : MonoBehaviour
{
    public GameObject mainCamera;
    int startFrame;

    public AudioSource returnSound;
    public AudioSource BGMSound;
    IEnumerator BGMRoutine;

    public Button returnButton;

    void Start()
    {
        startFrame = Time.frameCount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(ShowScreen());
        BGMRoutine = StartBGM();
        StartCoroutine(BGMRoutine);
    }
    IEnumerator ShowScreen()
    {
        GlobalVariable.TAKING_INPUT = false;
        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(2f);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GlobalVariable.TAKING_INPUT = true;
    }
    IEnumerator StartBGM()
    {
        BGMSound.Play();
        float lerpAmount = 0f;
        while(lerpAmount < 0.4f)
        {
            lerpAmount += 0.1f*Time.deltaTime;
            BGMSound.volume = lerpAmount;
            yield return null;
        }
    }
    IEnumerator StopBGM()
    {
        float lerpAmount = BGMSound.volume;
        while(lerpAmount > 0f)
        {
            lerpAmount -= 0.2f*Time.deltaTime;
            BGMSound.volume = lerpAmount;
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

    public void ToIntroduction()
    {
        if(GlobalVariable.TAKING_INPUT)
        {
            StartCoroutine(C_ToIntroduction());
        }

    }
    IEnumerator C_ToIntroduction()
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        returnSound.Play();
        StopCoroutine(BGMRoutine);
        BGMRoutine = StopBGM();
        StartCoroutine(BGMRoutine);
        ColorBlock temp = returnButton.colors;
        temp.normalColor = new Color(1f,0f,0f,1f);
        returnButton.colors = temp;

        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Introduction");
    }

}
