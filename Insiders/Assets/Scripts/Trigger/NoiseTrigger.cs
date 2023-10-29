using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTrigger : MonoBehaviour
{
    public AudioSource noiseSound;

    void OnTriggerEnter(Collider other)
    {
        if(noiseSound.isPlaying)
        {
            return;
        }
        if(other.transform.CompareTag("Player"))
        {
            noiseSound.Play();
            StartCoroutine(DestroyAfterPlay());
        }
    }

    IEnumerator DestroyAfterPlay()
    {
        while(noiseSound.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
