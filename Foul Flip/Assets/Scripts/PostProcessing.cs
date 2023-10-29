using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PostProcessing : MonoBehaviour
{
    public static PostProcessing postProcessing;

    private bool shouldBlink;
    private float blinkIntensity; // from 0 to 100
    private float blinkSpeedMin = 0.1f;
    private float blinkSpeedMax = 0.25f;
    private float blinkWeightMin = 0.75f;
    private float blinkWeightMax = 0.9f;
    private float blinkWeightRange = 0.1f;
    public Volume blinkVoulme;

    private void Awake()
    {
        if(null != postProcessing)
        {
            Destroy(gameObject);
            return;
        }
        postProcessing = this;
        DontDestroyOnLoad(gameObject);

        shouldBlink = false;
    }

    private void Start()
    {
        StartCoroutine(BlinkCoroutine());
    }

    public void StartBlink()
    {
        shouldBlink = true;
    }
    public void StopBlink()
    {
        shouldBlink = false;
    }

    public void AddBlinkIntensityBy(float value)
    {
        blinkIntensity += value;
        blinkIntensity = Mathf.Min(100f, blinkIntensity);
        blinkIntensity = Mathf.Max(1f, blinkIntensity);
    }

    private IEnumerator BlinkCoroutine()
    {
        while(true)
        {
            if(!shouldBlink)
            {
                yield return null;
            }

            float currentBlinkSpeed = Mathf.Lerp(blinkSpeedMin, blinkSpeedMax, blinkIntensity/100f);
            float currentblinkWeight = Mathf.Lerp(blinkWeightMin, blinkWeightMax, blinkIntensity/100f) + blinkWeightRange;

            while(blinkVoulme.weight < currentblinkWeight)
            {
                blinkVoulme.weight += currentBlinkSpeed*Time.deltaTime;
                yield return null;
            }
            while(blinkVoulme.weight > blinkWeightMin)
            {
                blinkVoulme.weight -= currentBlinkSpeed*Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}
