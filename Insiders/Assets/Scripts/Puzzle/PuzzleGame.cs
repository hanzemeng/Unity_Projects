using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleGame : MonoBehaviour
{
    protected abstract bool CheckWin();
    protected abstract void Win();

    protected void Update()
    {   
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ReturnToGameView());
        }
    }

    protected IEnumerator ReturnToGameView()
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Hint.hint.ShowHint("");
        PostProcessing.postProcessing.FadeToBlack();
        yield return new WaitForSeconds(2f);
        
        PlayerTransition.playerTransition.EnablePlayerCamera();
        PostProcessing.postProcessing.FadeToWhite();
        for(int i=gameObject.transform.childCount-1; i>=0; i--)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(1f);
        PlayerTransition.playerTransition.ShowPlayerUI();
        gameObject.SetActive(false);
        GlobalVariable.TAKING_INPUT = true;
    }
}
