using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    public GameObject hintText;
    public static Hint hint;
    IEnumerator currentHint;
    void Awake()
    {
        hint = this;
        currentHint = C_ShowHint("");
    }

    public enum TutorialMessage
    {
        T_MOVE,
        T_MOUSE,
        T_PUZZLE,
        T_ITEM,
        T_HIDE,
        T_RETURN
    }
    const int tutorialCount = 6;
    bool[] tutorialDisplayed = new bool[tutorialCount];

    void Start()
    {
        for(int i=0; i<tutorialCount; i++)
        {
            tutorialDisplayed[i] = !GameSettings.showTutorial;
        }
        StartCoroutine(ShowMoveTutorial());
    }

    IEnumerator ShowMoveTutorial()
    {
        yield return new WaitForSeconds(5f);
        ShowTutorial(TutorialMessage.T_MOVE);
        yield return new WaitForSeconds(3f);
        ShowTutorial(TutorialMessage.T_MOUSE);
    }

    public void ShowTutorial(TutorialMessage tutorialMessage)
    {
        switch(tutorialMessage)
        {
            case TutorialMessage.T_MOVE:
            {
                if(!tutorialDisplayed[0])
                {
                    tutorialDisplayed[0] = true;
                    ShowHint("hold w/a/s/d to move.");
                }
                break;
            }
            case TutorialMessage.T_MOUSE:
            {
                if(!tutorialDisplayed[1])
                {
                    tutorialDisplayed[1] = true;
                    ShowHint("move your mouse to look around. left click to interact.");
                }
                break;
            }
            case TutorialMessage.T_PUZZLE:
            {
                if(!tutorialDisplayed[2])
                {
                    tutorialDisplayed[2] = true;
                    ShowHint("when solving a puzzle, you may right click to return to the game view.");
                }
                break;
            }
            case TutorialMessage.T_ITEM:
            {
                if(!tutorialDisplayed[3])
                {
                    tutorialDisplayed[3] = true;
                    ShowHint("press q/e to select different items. left click to use the selected item.");
                }
                break;
            }
            case TutorialMessage.T_HIDE:
            {
                if(!tutorialDisplayed[4])
                {
                    tutorialDisplayed[4] = true;
                    ShowHint("you may hide under a bed to calm down. right click to stop hiding.");
                }
                break;
            }
            case TutorialMessage.T_RETURN:
            {
                if(!tutorialDisplayed[5])
                {
                    tutorialDisplayed[5] = true;
                    ShowHint("you may press left shift + p to return to the game menu.\n you current progress will be lost.");
                }
                break;
            }
            default: break;
        }
    }

    public void ShowHint(string hintMessage)
    {
        StopCoroutine(currentHint);
        currentHint = C_ShowHint(hintMessage);
        StartCoroutine(currentHint);
    }
    IEnumerator C_ShowHint(string hintMessage)
    {
        hintText.GetComponent<Text>().text = hintMessage;
        hintText.SetActive(true);
        yield return new WaitForSeconds(3f);
        hintText.SetActive(false);
    }
}
