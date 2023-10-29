using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject interactionOption;

    private Image crosshairImage;
    private Text interactionOptionText;

    void Awake()
    {
        crosshairImage = crosshair.GetComponent<Image>();
        interactionOptionText = interactionOption.GetComponent<Text>();
    }

    public void SetCrosshairImageColor(Color color)
    {
        crosshairImage.color = color;
        if(0f == color.a)
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }
    }
    public void SetInteractionOptionText(string text)
    {
        interactionOptionText.text = text;
        if("" == text)
        {
            interactionOption.SetActive(false);
        }
        else
        {
            interactionOption.SetActive(true);
        }
    }
}
