using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopNoiseTriggerOpenClose : MonoBehaviour
{
    public AudioSource noise;

    public void ObjectClicked()
    {
        StartCoroutine(FadeOutVolume());
    }

    IEnumerator FadeOutVolume()
    {
        AudioSource noiseSound = noise.GetComponent<AudioSource>();
        float volume = noiseSound.volume;
        while(volume>0f)
        {
            volume -= 1f*Time.deltaTime;
            noiseSound.volume = volume;
            yield return null;
        }
        Destroy(noise);
        Destroy(GetComponent<StopNoiseTriggerOpenClose>());
    }
}
