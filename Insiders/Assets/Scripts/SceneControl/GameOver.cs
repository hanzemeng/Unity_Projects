using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject jumpScare;
    public Image jumpScareFade;
    public AudioSource jumpScareSound;
    public GameObject player;
    public GameObject puzzle;
    public GameObject hide;
    public GameObject hint;
    private PlayerStress playerStress;

    public float deathValue;
    public float deathStartTime;

    void Start()
    {
        playerStress = player.GetComponent<PlayerStress>();

        deathValue = 0;

        if(GameSettings.isInEasyMode)
        {
            deathStartTime = 600f;
        }
    }

    void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
        {
            GlobalVariable.TAKING_INPUT = false;
            if(deathValue<200f)
            {
                StartCoroutine(ReturnToIntroduction());
            }
            else
            {
                StartCoroutine(Over());
            }
            return;
        }

        if(Timer.timer.GetElapsedGameTime()<deathStartTime)
        {
            return;
        }
        
        deathValue += (2f + Mathf.Min(4f, (float)GlobalVariable.OPEN_VALUE/5f) - 2*PlayerData.hideValue)*Time.deltaTime;
        if(deathValue<50f)
        {
            playerStress.SetStressLevel(0);
        }
        else if(deathValue<150f)
        {
            playerStress.SetStressLevel(1);
        }
        else if(deathValue<200f)
        {
            playerStress.SetStressLevel(2);
        }
        else if(deathValue<250f)
        {
            playerStress.SetStressLevel(3);
        }
        else
        {
            GlobalVariable.TAKING_INPUT = false;
            StartCoroutine(Over());
        }
        Debug.Log(deathValue);
    }

    IEnumerator Over()
    {
        Destroy(puzzle);
        Destroy(hide);
        Destroy(hint);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        PlayerTransition.playerTransition.EnablePlayerCamera();
        PlayerTransition.playerTransition.HidePlayerUI();
        player.GetComponent<PlayerView>().FadeToRed();
        playerStress.SetStressLevel(0);
        yield return new WaitForSeconds(0.25f);
        Destroy(player);
        jumpScareSound.Play();
        yield return new WaitForSeconds(0.1f);
        jumpScare.SetActive(true);

        Color startColor = new Color(1f,0f,0f,1f);
        Color endColor = new Color(1f,0f,0f,0f);
        float lerpAmount = 0f;
        while(lerpAmount<1f)
        {
            lerpAmount += 4*Time.deltaTime;
            jumpScareFade.color = Color.Lerp(startColor, endColor, lerpAmount);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        jumpScareSound.Stop();

        PostProcessing.postProcessing.InstantlyToBlack();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Introduction");
    }

    IEnumerator ReturnToIntroduction()
    {
        Destroy(hint);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        PlayerTransition.playerTransition.HidePlayerUI();
        playerStress.SetStressLevel(0);
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Introduction");
    }
}
