using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertUI : MonoBehaviour
{
    public static AlertUI alertUI;

    public CanvasGroup canvasGroup;
    public float canvasGroupFadeTime;

    public TMP_Text removeAdsPriceText;

    public TMP_Text loadingText;
    private bool shouldShowLoadingText;

    private void Awake()
    {
        if(null != alertUI)
        {
            Destroy(gameObject);
            return;
        }
        alertUI = this;
        DontDestroyOnLoad(gameObject);
    }

    public float FadeInCanvasGroup()
    {
        StartCoroutine(FadeInCanvasGroupCoroutine());
        return canvasGroupFadeTime;
    }
    private IEnumerator FadeInCanvasGroupCoroutine()
    {
        canvasGroup.transform.GetComponent<Canvas>().sortingOrder = 2;
        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / canvasGroupFadeTime;
            canvasGroup.alpha = Mathf.Lerp(0f,1f,lerpAmount);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public float FadeOutCanvasGroup()
    {
        StartCoroutine(FadeOutCanvasGroupCoroutine());
        return canvasGroupFadeTime;
    }
    private IEnumerator FadeOutCanvasGroupCoroutine()
    {
        canvasGroup.transform.GetComponent<Canvas>().sortingOrder = 1;
        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += Time.deltaTime / canvasGroupFadeTime;
            canvasGroup.alpha = Mathf.Lerp(1f,0f,lerpAmount);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public void SoldRemoveAdsText()
    {
        removeAdsPriceText.text = "SOLD";
    }

    public void ShowLoadingText()
    {
        shouldShowLoadingText = true;
        StartCoroutine(ShowLoadingTextCoroutine());
    }
    public void HideLoadingText()
    {
        loadingText.text = "";
        shouldShowLoadingText = false;
    }
    private IEnumerator ShowLoadingTextCoroutine()
    {
        string originalText = "LOADING...";
        while(true)
        {
            for(int i=originalText.Length-3; i<=originalText.Length; i++)
            {
                if(!shouldShowLoadingText)
                {
                    yield break;
                }

                loadingText.text = originalText.Insert(i, "<color=#ffffff00>");
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
