using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoPuzzle : PuzzleGame
{
    public AudioSource note;
    public int solutionOrder;
    int currentOrder;

    public GameObject beforePiano;
    public GameObject afterPiano;
    public PuzzleDoor secondFloorDoorLock;
    public AudioSource doorOpenSound;

    public void PlayNote(int noteNumber)
    {
        note.pitch = (float) noteNumber * 0.1f + 0.5f;
        note.Play();
        currentOrder = 10*currentOrder+noteNumber;
        currentOrder %= 100000;
        if(CheckWin())
        {
            Win();
        }
    }

    protected override bool CheckWin()
    {
        return solutionOrder == currentOrder;
    }
    protected override void Win()
    {
        doorOpenSound.Play();
        beforePiano.SetActive(false);
        afterPiano.SetActive(true);
        Destroy(secondFloorDoorLock);
        StartCoroutine(ReturnToGameView());
    }
}
