using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseLightsOffTrigger : MonoBehaviour
{
    public GameObject[] disableTrigger;
    public GameObject[] disableLights;
    public GameObject playerLight;
    public float timeToTrigger;
    bool isTriggered;

    public AudioSource triggerSound;

    void Start()
    {
        isTriggered = false;
        if(GameSettings.isInEasyMode)
        {
            timeToTrigger = 630f;
        }
    }

    void Update()
    {
        if(Timer.timer.GetElapsedGameTime()>timeToTrigger && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(TurnOffLights());
            
        }
    }

    IEnumerator TurnOffLights()
    {
        triggerSound.Play();
        yield return new WaitForSeconds(7f);

        for(int i=disableTrigger.Length-1; i>=0; i--)
        {
            Destroy(disableTrigger[i]);
        }

        for(int i=disableLights.Length-1; i>=0; i--)
        {
            for(int j=disableLights[i].transform.childCount-1; j>=0; j--)
            {
                disableLights[i].transform.GetChild(j).GetComponent<LightSwitch>().LightOff();
                yield return new WaitForSeconds(0.05f);
            }
        }
        playerLight.SetActive(true);
        Destroy(gameObject);
    }
}
