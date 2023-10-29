using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStress : MonoBehaviour
{
    public int stressLevel;
    public PlayerSound playerSound;

    void Start()
    {
        stressLevel = 0;
        StartCoroutine(HeartBeatSound());
    }

    public void SetStressLevel(int val)
    {
        if(stressLevel == val)
        {
            return;
        }
        stressLevel = val;
        stressLevel = Mathf.Max(0, stressLevel);
        stressLevel = Mathf.Min(3, stressLevel);
        switch(stressLevel) 
        {
            case 0:
                PostProcessing.postProcessing.StopBlink();
                playerSound.StopLoopScareSound();
                break;
            case 1:
                PostProcessing.postProcessing.Blink(1f, 0.4f);
                playerSound.LoopScareSound(0.4f);
                Hint.hint.ShowTutorial(Hint.TutorialMessage.T_HIDE);
                break;
            case 2:
                PostProcessing.postProcessing.Blink(0.75f, 0.6f);
                playerSound.LoopScareSound(0.6f);
                Hint.hint.ShowTutorial(Hint.TutorialMessage.T_RETURN);
                break;
            case 3:
                PostProcessing.postProcessing.Blink(0.6f, 0.8f);
                playerSound.LoopScareSound(0.8f);
                break;
        }
    }

    IEnumerator HeartBeatSound()
    {
        while(true)
        {
            switch(stressLevel) 
            {
            case 0:
                yield return new WaitForSeconds(1f);
                break;
            case 1:
                playerSound.PlayHeartBeatSound();
                yield return new WaitForSeconds(1f);
                break;
            case 2:
                playerSound.PlayHeartBeatSound();
                yield return new WaitForSeconds(0.75f);
                break;
            case 3:
                playerSound.PlayHeartBeatSound();
                yield return new WaitForSeconds(0.6f);
                break;
            }
        }
    }
}
