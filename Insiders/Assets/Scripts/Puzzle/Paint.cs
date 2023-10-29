using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    public PaintPuzzle paintPuzzle;
    public int currentY;
    public int currentX;
    public int solutionY;
    public int solutionX;
    

    void OnMouseDown()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        paintPuzzle.Move(gameObject);
    }
}
