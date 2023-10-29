using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNoiseTrigger : MonoBehaviour
{
    public GameObject noise;

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            noise.GetComponent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }
}
