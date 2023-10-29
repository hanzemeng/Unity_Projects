using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafePuzzle : PuzzleGame
{
    public GameObject[] rullers = new GameObject[4];
    public int[] solutionArray = new int[4];
    private int[] numberArray = {0,0,0,0};
    bool isRotating;

    public GameObject closedBox;
    public GameObject openedBox;
    public GameObject basementFirstFloorNoiseTrigger;

    public AudioSource rollerRotateSound;
    public AudioSource unlockSound;
    void Start()
    {
        isRotating = false;
        foreach (GameObject r in rullers)
        {
            r.transform.Rotate(-144f, 0, 0, Space.Self);
        }
    }

    public void RotateRullers(int val)
    {
        if(isRotating)
        {
            return;
        }

        rollerRotateSound.Play();
        if(val>0)
        {
            val--;
            StartCoroutine(Rotate(rullers[val].transform, 36f));
            //rullers[val].transform.Rotate(36f, 0, 0, Space.Self);

            numberArray[val] -= 1;
            if (numberArray[val] < 0)
            {
                numberArray[val] = 9;
            }
        }
        else
        {
            val = -1*val - 1;
            StartCoroutine(Rotate(rullers[val].transform, -36f));
            //rullers[val].transform.Rotate(-36f, 0, 0, Space.Self);

            numberArray[val] += 1;
            if (numberArray[val] > 9)
            {
                numberArray[val] = 0;
            }
        }

        
    }

    IEnumerator Rotate(Transform obj, float degree)
    {
        isRotating = true;

        Quaternion originalRotation = obj.localRotation;

        float rotateTime = 0;
        while(rotateTime<0.1f)
        {
            rotateTime += Time.deltaTime;
            obj.Rotate(10f*degree*Time.deltaTime, 0, 0, Space.Self);
            yield return null;
        }
        
        obj.localRotation = originalRotation;
        obj.transform.Rotate(degree, 0, 0, Space.Self);

        isRotating = false;
        if(CheckWin())
        {
            Win();
        }
    }

    protected override bool CheckWin()
    {
        for(int i=0; i<4; i++)
        {
            if(numberArray[i] != solutionArray[i])
            {
                return false;
            }
        }
        return true;
    }

    protected override void Win()
    {
        unlockSound.Play();
        Destroy(closedBox);
        openedBox.SetActive(true);
        basementFirstFloorNoiseTrigger.SetActive(true);
        StartCoroutine(ReturnToGameView());
    }
}
