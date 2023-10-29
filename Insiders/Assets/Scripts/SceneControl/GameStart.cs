using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject mainCamera;
    public PlayerTransition playerTransition;
    int startFrame;

    public AudioSource startAudio;

    void Awake()
    {
        GlobalVariable.OPEN_VALUE = 0;
        PlayerData.LoadData();
        Time.timeScale = 1f;
    }

    void Start()
    {
        playerTransition.HidePlayerUI();
        startFrame = Time.frameCount;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(WakeUp());
    }

    void Update()
    {
        if(Time.frameCount-startFrame > 2)
        {
            mainCamera.SetActive(true);
        }
    }


    IEnumerator WakeUp()
    {
        GlobalVariable.TAKING_INPUT = false;
        PostProcessing.postProcessing.WakeUp();
        startAudio.Play();
        float lerpAmount = 1f;
        while(lerpAmount>0f)
        {
            lerpAmount -= 0.25f*Time.deltaTime;
            startAudio.volume = lerpAmount;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        GlobalVariable.TAKING_INPUT = true;
        playerTransition.ShowPlayerUI();
        Destroy(gameObject);
    }
}
