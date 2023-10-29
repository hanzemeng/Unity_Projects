using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOnTrigger : MonoBehaviour
{
    public GameObject lights;
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            for(int i=lights.transform.childCount-1; i>=0; i--)
            {
                lights.transform.GetChild(i).GetComponent<LightSwitch>().LightOn();
            }
        }
    }
}
