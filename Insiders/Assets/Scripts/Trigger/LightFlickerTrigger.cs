using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerTrigger : MonoBehaviour
{
    public GameObject light;
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            light.GetComponent<LightSwitch>().Flicker();
        }
        Destroy(gameObject);
    }
}
