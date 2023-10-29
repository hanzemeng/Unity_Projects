using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideCamera : MonoBehaviour
{
    public Text hideText;
    bool textRevealed;

    public float lookSpeed;

    void Start()
    {
        textRevealed = false;
    }

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    public void RevealHideText()
    {
        if(!textRevealed)
        {
            textRevealed = true;
            StartCoroutine(C_RevealHideText());
        }
    }

    IEnumerator C_RevealHideText()
    {
        Color startColor = new Color(1f,1f,1f,0f);
        Color endColor = new Color(1f,1f,1f,1f);
        float lerpAmount = 0f;
        while(lerpAmount<1f)
        {
            lerpAmount += 2f*Time.deltaTime;
            hideText.color = Color.Lerp(startColor, endColor, lerpAmount);
            yield return null;
        }
    }
}
