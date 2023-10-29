using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public int currentIndex;
    public int solutionIndex;
    public Camera camera;
    public BookShelfPuzzle bookShelfPuzzle;
    void OnMouseDrag()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if(null != hit.transform.GetComponent<Book>())
            {
                bookShelfPuzzle.Swap(gameObject, hit.transform.gameObject);
            }
        }
    }

    void OnMouseUp()
    {
        if(!GlobalVariable.TAKING_INPUT)
        {
            return;
        }
        bookShelfPuzzle.CheckWinByBook();
    }
}
