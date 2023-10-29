using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKey : MonoBehaviour
{
    public PianoPuzzle pianoPuzzle;
    public int key;
    void OnMouseDown()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        pianoPuzzle.PlayNote(key);
    }
}
