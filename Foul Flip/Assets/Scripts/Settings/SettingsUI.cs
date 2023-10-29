using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    public static SettingsUI settingsUI;

    public CanvasGroup canvasGroup;
    public float canvasGroupFadeTime;

    public List<Image> dimensionOptions;
    public List<Image> rangeOptions;

    public TMP_InputField seedText;

    private void Awake()
    {
        if(null != settingsUI)
        {
            Destroy(gameObject);
            return;
        }
        settingsUI = this;
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

    public void HighlightDimensionOption(int optionIndex)
    {
        foreach(Image option in dimensionOptions)
        {
            option.color = new Color(1f,1f,1f,1f);
        }
        dimensionOptions[optionIndex].color = new Color(0f,1f,0f,1f);
    }

    public void HighlightRangeOption(int optionIndex)
    {
        foreach(Image option in rangeOptions)
        {
            option.color = new Color(1f,1f,1f,1f);
        }
        rangeOptions[optionIndex].color = new Color(0f,1f,0f,1f);
    }

    public void ChangeSeedText(string newText)
    {
        seedText.text = newText;
    }
}
