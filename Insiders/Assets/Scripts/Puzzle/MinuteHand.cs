using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinuteHand : MonoBehaviour
{
    float originX = 962f;
    float originY = 497f;

    int minuteValue;
    public ClockPuzzle clockPuzzle;

    public AudioSource turnSound;

    void OnMouseDrag()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        float angleDeg = 180f/Mathf.PI * Mathf.Atan2(mouseX-originX, mouseY-originY);
        if(angleDeg<0f)
        {
            angleDeg += 360f;
        }

        int newMinuteValue = (int)angleDeg/6;
        if(newMinuteValue != minuteValue)
        {
            turnSound.Play();
            minuteValue = newMinuteValue;
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 6f* (float) minuteValue);
        }
    }

    void OnMouseUp()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        clockPuzzle.minuteValue = minuteValue;
        clockPuzzle.ClockHandCheckWin();
    }
}
