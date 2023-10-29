using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSystem : MonoBehaviour
{
    public HideCamera hideCamera;
    public float hideValue;

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        hideValue -= 2f*Time.deltaTime;
        hideValue = Mathf.Max(0f, hideValue);
        PlayerData.hideValue = hideValue;

        if(hideValue < 2f)
        {
            hideCamera.RevealHideText();
        }
        if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ReturnToGameView());
        }
    }

    IEnumerator ReturnToGameView()
    {
        PlayerData.hideValue = 0;
        GlobalVariable.TAKING_INPUT = false;
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);

        PlayerTransition.playerTransition.EnablePlayerCamera();
        transform.GetChild(0).gameObject.SetActive(false);
        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(1f);

        PlayerTransition.playerTransition.ShowPlayerUI();
        GlobalVariable.TAKING_INPUT = true;
        gameObject.SetActive(false);
    }
}
