using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public bool isOn;
    public GameObject on;
    public GameObject off;
    public AudioSource sound;

    private IEnumerator flickerRoutine;

    void Start()
    {
        flickerRoutine = C_Flicker();
    }

    public void LightOn()
    {
        if(null != flickerRoutine)
        {
            StopCoroutine(flickerRoutine);
        }
        
        if(!isOn)
        {
            on.SetActive(true);
            off.SetActive(false);
            sound.Play();
            isOn = true;
        }
    }
    public void LightOff()
    {
        if(null != flickerRoutine)
        {
            StopCoroutine(flickerRoutine);
        }
        if(isOn)
        {
            on.SetActive(false);
            off.SetActive(true);
            sound.Play();
            isOn = false;
        }
    }
    public void LightOnNoSound()
    {
        if(null != flickerRoutine)
        {
            StopCoroutine(flickerRoutine);
        }
        if(!isOn)
        {
            on.SetActive(true);
            off.SetActive(false);
            isOn = true;
        }
    }
    public void LightOffNoSound()
    {
        if(null != flickerRoutine)
        {
            StopCoroutine(flickerRoutine);
        }
        if(isOn)
        {
            on.SetActive(false);
            off.SetActive(true);
            isOn = false;
        }
    }

    public void Flicker()
    {
        StartCoroutine(flickerRoutine);
    }

    IEnumerator C_Flicker()
    {
        bool originalState = isOn;
        for(int i=Random.Range(3, 8); i>=0; i--)
        {
            if(isOn)
            {
                on.SetActive(false);
                off.SetActive(true);
                isOn = false;
            }
            else
            {
                on.SetActive(true);
                off.SetActive(false);
                isOn = true;
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));
        }

        if(originalState != isOn)
        {
            if(isOn)
            {
                on.SetActive(false);
                off.SetActive(true);
                isOn = false;
            }
            else
            {
                on.SetActive(true);
                off.SetActive(false);
                isOn = true;
            }
        }
    }
}
