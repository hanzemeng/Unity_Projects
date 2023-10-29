using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockPuzzle : PuzzleGame
{
    public int solutionHourValue;
    public int solutionMinuteValue;
    public int hourValue;
    public int minuteValue;

    public GameObject clockBefore;
    public GameObject clockAfter;
    public PuzzleGlassDoor cabinetLockLeft;
    public PuzzleGlassDoor cabinetLockRight;

    public AudioSource unlockSound;

    public void ClockHandCheckWin()
    {
        if(CheckWin())
        {
            Win();
        }
    }
    protected override bool CheckWin()
    {
        return solutionHourValue == hourValue && solutionMinuteValue == minuteValue;
    }
    protected override void Win()
    {
        unlockSound.Play();
        clockBefore.SetActive(false);
        clockAfter.SetActive(true);
        Destroy(cabinetLockLeft);
        Destroy(cabinetLockRight);
        StartCoroutine(ReturnToGameView());
    }
}
