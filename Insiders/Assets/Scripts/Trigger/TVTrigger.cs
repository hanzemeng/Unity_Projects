using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVTrigger : MonoBehaviour
{
    public LightSwitch TV;

    bool hasTriggered;

    void Start()
    {
        hasTriggered = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(hasTriggered)
        {
            return;
        }

        if(other.transform.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(TVScare());
        }
    }
    IEnumerator TVScare()
    {
        TV.LightOnNoSound();
        TV.sound.Play();
        yield return new WaitForSeconds(1.5f);
        TV.sound.Stop();
        TV.LightOffNoSound();
        Destroy(gameObject);
    }
}
