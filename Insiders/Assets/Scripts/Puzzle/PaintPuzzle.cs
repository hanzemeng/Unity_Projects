using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintPuzzle : PuzzleGame
{
    public Paint[] paints;
    bool[,] isEmpty = new bool[3, 3];
    bool hasPaintMoving;
    public AudioSource movePaintSound;

    public GameObject beforePaint;
    public GameObject afterPaint;
    public LightSwitch lightSwitch;
    public CandleUnlightable candleUnlightable;

    public GameObject zombieTrigger;
    public GameObject bloodTrigger;

    void Start()
    {
        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                isEmpty[i, j] = false;
            }
        }
        isEmpty[2, 0] = true;
        hasPaintMoving = false;
    }

    public void Move(GameObject paint)
    {
        if(hasPaintMoving)
        {
            return;
        }

        float moveAmount = 0.08f;
        int currentX = paint.GetComponent<Paint>().currentX;
        int currentY = paint.GetComponent<Paint>().currentY;

        if(currentX-1>=0 && isEmpty[currentY, currentX-1])
        {
            isEmpty[currentY, currentX] = true;
            isEmpty[currentY, currentX-1] = false;
            paint.GetComponent<Paint>().currentX--;
            StartCoroutine(MovePaint(paint, new Vector3(0f,0f, moveAmount)));
        }
        else if(currentX+1<3 && isEmpty[currentY, currentX+1])
        {
            isEmpty[currentY, currentX] = true;
            isEmpty[currentY, currentX+1] = false;
            paint.GetComponent<Paint>().currentX++;
            StartCoroutine(MovePaint(paint, new Vector3(0f,0f, -moveAmount)));
        }
        else if(currentY-1>=0 && isEmpty[currentY-1, currentX])
        {
            isEmpty[currentY, currentX] = true;
            isEmpty[currentY-1, currentX] = false;
            paint.GetComponent<Paint>().currentY--;
            StartCoroutine(MovePaint(paint, new Vector3(0f,moveAmount, 0f)));
        }
        else if(currentY+1<3 && isEmpty[currentY+1, currentX])
        {
            isEmpty[currentY, currentX] = true;
            isEmpty[currentY+1, currentX] = false;
            paint.GetComponent<Paint>().currentY++;
            StartCoroutine(MovePaint(paint, new Vector3(0f,-moveAmount, 0f)));
        }
    }

    IEnumerator MovePaint(GameObject paint, Vector3 moveAmount)
    {
        movePaintSound.Play();
        hasPaintMoving = true;
        float lerpAmount = 0f;
        Vector3 startLocation = paint.transform.localPosition;
        Vector3 endLocation = startLocation + moveAmount;
        while(lerpAmount<1f)
        {
            lerpAmount += 8f*Time.deltaTime;
            paint.transform.localPosition = Vector3.Lerp(startLocation, endLocation, lerpAmount);
            yield return null;
        }
        paint.transform.localPosition = endLocation;
        hasPaintMoving = false;

        if(CheckWin())
        {
            Win();
        }
    }

    protected override bool CheckWin()
    {
        for(int i=0; i<8; i++)
        {
            if((paints[i].currentX != paints[i].solutionX) || (paints[i].currentY != paints[i].solutionY))
            {
                return false;
            }
        }
        return true;
    }
    protected override void Win()
    {
        StartCoroutine(ReturnToGameView());
        beforePaint.SetActive(false);
        afterPaint.SetActive(true);
        zombieTrigger.SetActive(true);
        bloodTrigger.SetActive(true);
        lightSwitch.LightOn();
        Destroy(candleUnlightable);
    }
}
