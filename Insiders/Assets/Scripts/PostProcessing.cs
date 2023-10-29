using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessing : MonoBehaviour
{
    public static PostProcessing postProcessing;
    void Awake()
    {
        postProcessing = this;
    }

    public Volume fade;
    public Volume wakeUp;
    public Volume blink;
    public Volume distortion;
    private IEnumerator blinkCoroutine;
    
    void Start()
    {
        fade.weight = 0f;
        wakeUp.weight = 0f;
    }

    public void InstantlyToBlack()
    {
        fade.weight = 1f;
    }
    public void FadeToBlack()
    {
        StartCoroutine(C_FadeToBlack());
    }
    IEnumerator C_FadeToBlack()
    {
        fade.weight = 0f;
        float lerpAmount = 0f;

        while(lerpAmount<1f)
        {
            lerpAmount += 0.5f*Time.deltaTime;
            fade.weight = lerpAmount;
            yield return null;
        }
    }
    public void FadeToWhite()
    {
        StartCoroutine(C_FadeToWhite());
    }
    IEnumerator C_FadeToWhite()
    {
        fade.weight = 1f;
        float lerpAmount = 1f;

        while(lerpAmount>0f)
        {
            lerpAmount -= 0.5f*Time.deltaTime;
            fade.weight = lerpAmount;
            yield return null;
        }
    }

    public void WakeUp()
    {
        StartCoroutine(C_WakeUp());
    }
    IEnumerator C_WakeUp()
    {
        wakeUp.weight = 0.9f;
        float lerpAmount = 0.9f;

        while(lerpAmount>0f)
        {
            lerpAmount -= 0.2f*Time.deltaTime;
            wakeUp.weight = lerpAmount;
            yield return null;
        }
    }

    public void StopBlink()
    {
        if(null != blinkCoroutine)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = C_StopBlink();
        StartCoroutine(blinkCoroutine);
    }
    IEnumerator C_StopBlink()
    {
        float lerpAmount = blink.weight;
        while(lerpAmount>0f)
        {
            lerpAmount -= 4f*Time.deltaTime;
            blink.weight = lerpAmount;
            yield return null;
        }
        blink.weight = 0f;
    }

    public void Blink(float period, float level)
    {
        if(null != blinkCoroutine)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = C_Blink(period, level);
        StartCoroutine(blinkCoroutine);
    }
    IEnumerator C_Blink(float period, float level)
    {
        float lerpAmount = blink.weight;
        while(lerpAmount<level)
        {
            lerpAmount += 2f*Time.deltaTime;
            blink.weight = lerpAmount;
            yield return null;
        }
        float speed = 2/(period*5);
        while(true)
        {
            while(lerpAmount<level+0.2f)
            {
                lerpAmount += speed*Time.deltaTime;
                blink.weight = lerpAmount;
                yield return null;
            }
            while(lerpAmount>level)
            {
                lerpAmount -= speed*Time.deltaTime;
                blink.weight = lerpAmount;
                yield return null;
            }
        }
    }

    public void StartDistortion()
    {
        distortion.weight = 1f;
    }
    public void StopDistortion()
    {
        distortion.weight = 0f;
    }
}
