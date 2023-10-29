using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public GameObject puzzleView;
    public string dispalyMessage;

    public string GetDisplayMessage()
    {
        return dispalyMessage;
    }

    public void LoadPuzzle()
    {
        if(puzzleView != null)
        {
            StartCoroutine(C_LoadPuzzle());
        }
    }
    IEnumerator C_LoadPuzzle()
    {
        GlobalVariable.TAKING_INPUT = false;
        PlayerTransition.playerTransition.HidePlayerUI();
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        
        PlayerTransition.playerTransition.DisablePlayerCamera();
        puzzleView.SetActive(true);
        for(int i=puzzleView.transform.childCount-1; i>=0; i--)
        {
            puzzleView.transform.GetChild(i).gameObject.SetActive(true);
        }

        PostProcessing.postProcessing.FadeToWhite();
        yield return new WaitForSeconds(2f);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GlobalVariable.TAKING_INPUT = true;
    }
}
