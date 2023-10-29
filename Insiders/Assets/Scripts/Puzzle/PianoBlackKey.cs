using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoBlackKey : MonoBehaviour
{
    void OnMouseDown()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        Hint.hint.ShowHint("the black keys are broken");
    }
}
