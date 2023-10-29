using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MagicCircleEndControl : MonoBehaviour
{
    public Image fadeImage;

    public GameObject text;
    IEnumerator currentTransition;
    float timer;


    bool isReturning;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        timer = 0f;
        currentTransition = SceneTransition();
        StartCoroutine(currentTransition);

        isReturning = false;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(Input.GetMouseButtonDown(1) && timer > 4f && !isReturning)
        {
            isReturning = true;
            StopCoroutine(currentTransition);
            currentTransition = QuickTransition();
            StartCoroutine(currentTransition);
        }
    }

    IEnumerator SceneTransition()
    {
        yield return new WaitForSeconds(2f);
        Color startColor = new Color(1f,1f,1f,1f);
        Color endColor = new Color(1f,1f,1f,0f);
        float lerpAmount = 0f;
        while(lerpAmount<1f)
        {
            lerpAmount += 0.5f*Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, lerpAmount);
            yield return null;
        }

        for(int i=0; i<text.transform.childCount; i++)
        {
            Text currentText = text.transform.GetChild(i).GetComponent<Text>();
            startColor = currentText.color;
            endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
            lerpAmount = 0f;
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
