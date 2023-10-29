using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : MonoBehaviour
{
    public GameObject hideSystem;

    public void LoadHide()
    {
        StartCoroutine(C_LoadHide());
    }

    IEnumerator C_LoadHide()
    {
        GlobalVariable.TAKING_INPUT = false;
        PlayerTransition.playerTransition.HidePlayerUI();
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);

        PlayerTransition.playerTransition.DisablePlayerCamera();
        hideSystem.SetActive(true);
        hideSystem.transform.GetChild(0).gameObject.SetActive(true);

        GlobalVariable.TAKING_INPUT = true;
        PostProcessing.postProcessing.FadeToWhite();
    }

}
