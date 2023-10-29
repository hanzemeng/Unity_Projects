using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfPuzzle : PuzzleGame
{
    public Book[] books;

    public GameObject beforeBookShelf;
    public GameObject afterBookShelf;
    public LightSwitch lightSwitch;
    public CandleUnlightable candleUnlightable;

    public AudioSource moveBookSound;

    public GameObject hallWayTrigger;

    public void Swap(GameObject a, GameObject b)
    {
        if(a == b)
        {
            return;
        }
        a.GetComponent<Book>().currentIndex += b.GetComponent<Book>().currentIndex;
        b.GetComponent<Book>().currentIndex = a.GetComponent<Book>().currentIndex - b.GetComponent<Book>().currentIndex;
        a.GetComponent<Book>().currentIndex -= b.GetComponent<Book>().currentIndex;

        a.transform.localPosition += b.transform.localPosition;
        b.transform.localPosition = a.transform.localPosition - b.transform.localPosition;
        a.transform.localPosition -= b.transform.localPosition;

        moveBookSound.Play();
    }


    public void CheckWinByBook()
    {
        if(CheckWin())
        {
            Win();
        }
    }
    protected override bool CheckWin()
    {
        for(int i=books.Length-1; i>=0; i--)
        {
            if(books[i].currentIndex != books[i].solutionIndex)
            {
                return false;
            }
        }
        return true;
    }
    protected override void Win()
    {
        StartCoroutine(ReturnToGameView());
        beforeBookShelf.SetActive(false);
        afterBookShelf.SetActive(true);
        lightSwitch.LightOn();
        Destroy(candleUnlightable);
        hallWayTrigger.SetActive(true);
    }
}
