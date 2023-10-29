using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourHand : MonoBehaviour
{
    float originX = 962f;
    float originY = 497f;

    int hourValue;
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
        int newHourValue = (int)angleDeg/30;
        if(newHourValue != hourValue)
        {
            turnSound.Play();
            hourValue = newHourValue;
            transform.localEulerAngles = new Vector3(0.0f, 0.0f, 30f* (float) hourValue);
        }
    }

    void OnMouseUp()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        clockPuzzle.hourValue = hourValue;
        clockPuzzle.ClockHandCheckWin();
    }
}
