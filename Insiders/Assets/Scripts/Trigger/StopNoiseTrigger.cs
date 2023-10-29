using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopNoiseTrigger : MonoBehaviour
{
    public AudioSource noise;

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            StartCoroutine(FadeOutVolume());
        }
    }

    IEnumerator FadeOutVolume()
    {
        if(null == noise)
        {
            Destroy(gameObject);
        }
        else
        {
            float volume = noise.volume;
            while(volume>0f)
            {
                volume -= 1f*Time.deltaTime;
                noise.volume = volume;
                yield return null;
            }
            Destroy(noise);
            Destroy(gameObject);
        }
    }
}
