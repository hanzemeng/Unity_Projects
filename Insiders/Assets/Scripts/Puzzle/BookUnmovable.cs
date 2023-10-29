using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookUnmovable : MonoBehaviour
{
    void OnMouseDrag()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
       Hint.hint.ShowHint("this book is glued to the shelf and can’t be moved");
    }
}
