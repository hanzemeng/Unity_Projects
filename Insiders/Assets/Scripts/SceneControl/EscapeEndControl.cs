using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EscapeEndControl : MonoBehaviour
{
    public GameObject mainCamera;
    int startFrame;

    public GameObject text;
    IEnumerator currentTransition;
    float timer;

    bool isReturning;

    void Start()
    {
        startFrame = Time.frameCount;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        timer = 0f;
        currentTransition = SceneTransition();
        StartCoroutine(currentTransition);

        isReturning = false;
    }

    void Update()
    {
        if(Time.frameCount-startFrame > 2)
        {
            mainCamera.SetActive(true);
        }
        
        timer += Time.deltaTime;
        if(Input.GetMouseButtonDown(1) && timer > 2f && !isReturning)
        {
            isReturning = true;
            StopCoroutine(currentTransition);
            currentTransition = QuickTransition();
            StartCoroutine(currentTransition);
        }
    }

    IEnumerator SceneTransition()
    {
        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(2f);

        for(int i=0; i<text.transform.childCount; i++)
        {
            Text currentText = text.transform.GetChild(i).GetComponent<Text>();
            Color startColor = currentText.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
            float lerpAmount = 0f;
            while(lerpAmount < 1f)
            {
                lerpAmount += 0.5f* Time.deltaTime;
                currentText.color = Color.Lerp(startColor, endColor, lerpAmount);
                yield return null;
            }
            yield return new WaitForSeconds(1.5f);
        }
        
        yield return new WaitForSeconds(1f);
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Introduction");
    }
    IEnumerator QuickTransition()
    {
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Introduction");
    }
}
