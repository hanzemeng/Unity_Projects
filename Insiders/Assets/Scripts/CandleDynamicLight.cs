using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleDynamicLight : MonoBehaviour
{
    public Light candleLight;
    public float minRange, maxRange, minTemperature, maxTemperature;

    void Start()
    {
        StartCoroutine(DynamicLight());
    }

    IEnumerator DynamicLight()
    {
        while(true)
        {
            float lerpAmount = 0f;

            float oriRange = candleLight.range;
            float oriTemperature = candleLight.colorTemperature;
            float newRange = Random.Range(minRange, maxRange);
            float newTemperature = Random.Range(minTemperature, maxTemperature);
            while(lerpAmount<1f)
            {
                lerpAmount += 5f*Time.deltaTime;
                candleLight.range = Mathf.Lerp(oriRange, newRange, lerpAmount);
                candleLight.colorTemperature = Mathf.Lerp(oriTemperature, newTemperature, lerpAmount);
                yield return null;
            }
        }
    }
}
