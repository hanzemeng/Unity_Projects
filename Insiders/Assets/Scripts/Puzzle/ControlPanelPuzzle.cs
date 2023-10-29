using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanelPuzzle : PuzzleGame
{
    bool[,] state = new bool[3, 3];
    public Image[] stateUI = new Image[9];

    public GameObject message1;
    public GameObject message2;

    public GameObject beforePhone;
    public GameObject afterPhone;
    public GameObject officeDoor;

    public AudioSource clickSound;
    public AudioSource winSound;
    void Start()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                state[i, j] = false;
            }
        }
        state[0, 2] = true;
        state[1, 1] = true;
    }

    protected new void Update()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ReturnToGameView(false));
        }
    }

    void UpdateUI()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                if(state[i,j])
                {
                    StartCoroutine(ChangeColor(stateUI[3*i+j], Color.green, 8f));
                }
                else
                {
                    StartCoroutine(ChangeColor(stateUI[3*i+j], Color.red, 8f));
                }
            }
        }
    }

    IEnumerator ChangeColor(Image img, Color goalColor, float speed)
    {
        Color oriColor = img.color;
        float lerpAmount = 0;

        while(lerpAmount<1f)
        {
            lerpAmount += speed*Time.deltaTime;
            img.color = Color.Lerp(oriColor, goalColor, lerpAmount);
            yield return null;
        }
    }
    IEnumerator ChangeColor(Text text, Color goalColor, float speed)
    {
        Color oriColor = text.color;
        float lerpAmount = 0;

        while(lerpAmount<1f)
        {
            lerpAmount += speed*Time.deltaTime;
            text.color = Color.Lerp(oriColor, goalColor, lerpAmount);
            yield return null;
        }
    }

    protected override bool CheckWin()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                if(!state[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }
    protected override void Win()
    {
        winSound.Play();
        beforePhone.SetActive(false);
        afterPhone.SetActive(true);
        Destroy(officeDoor.GetComponent<PuzzleDoor>());
        StartCoroutine(ReturnToGameView(true));
    }

    protected IEnumerator ReturnToGameView(bool showMessage)
    {
        GlobalVariable.TAKING_INPUT = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(showMessage)
        {
            yield return new WaitForSeconds(0.5f);
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    StartCoroutine(ChangeColor(stateUI[3*i+j], new Color(0f,1f,0f,0f), 1f));
                }
            }
            yield return new WaitForSeconds(1.5f);

            message1.SetActive(true);
            message2.SetActive(true);
            StartCoroutine(ChangeColor(message1.GetComponent<Text>(), new Color(1f,1f,1f,1f), 0.5f));
            yield return new WaitForSeconds(3f);
            StartCoroutine(ChangeColor(message2.GetComponent<Text>(), new Color(1f,0f,0f,1f), 0.5f));
            yield return new WaitForSeconds(3.5f);
        }

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
        if(showMessage)
        {
            officeDoor.BroadcastMessage("ObjectClicked");
        }
        gameObject.SetActive(false);
        GlobalVariable.TAKING_INPUT = true;
    }

    public void Button11()
    {
        clickSound.Play();
        state[0, 0] ^= true;
        state[0, 1] ^= true;
        state[1, 0] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button31()
    {
        clickSound.Play();
        state[2, 0] ^= true;
        state[2, 1] ^= true;
        state[1, 0] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button33()
    {
        clickSound.Play();
        state[2, 2] ^= true;
        state[2, 1] ^= true;
        state[1, 2] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button13()
    {
        clickSound.Play();
        state[0, 2] ^= true;
        state[1, 2] ^= true;
        state[0, 1] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button12()
    {
        clickSound.Play();
        state[0, 1] ^= true;
        state[0, 0] ^= true;
        state[0, 2] ^= true;
        state[1, 1] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button21()
    {
        clickSound.Play();
        state[1, 0] ^= true;
        state[0, 0] ^= true;
        state[1, 1] ^= true;
        state[2, 0] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button23()
    {
        clickSound.Play();
        state[1, 2] ^= true;
        state[0, 2] ^= true;
        state[1, 1] ^= true;
        state[2, 2] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button32()
    {
        clickSound.Play();
        state[2, 1] ^= true;
        state[2, 0] ^= true;
        state[1, 1] ^= true;
        state[2, 2] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
    public void Button22()
    {
        clickSound.Play();
        state[1, 1] ^= true;
        state[1, 0] ^= true;
        state[0, 1] ^= true;
        state[1, 2] ^= true;
        state[2, 1] ^= true;
        UpdateUI();
        if(CheckWin())
        {
            Win();
        }
    }
}
