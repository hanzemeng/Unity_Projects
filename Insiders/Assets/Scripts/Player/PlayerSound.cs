using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource walkSound;
    IEnumerator walkSoundRoutine;
    bool isWalking;
    public AudioSource heartBeatSound;
    public AudioSource scareSound;

    void Start()
    {
        isWalking = false;
    }

    public void PlayWalkSound()
    {
        if(!isWalking)
        {
            isWalking = true;
            if(null != walkSoundRoutine)
            {
                StopCoroutine(walkSoundRoutine);
            }
            walkSoundRoutine = C_PlayWalkSound();
            StartCoroutine(walkSoundRoutine);
        }
    }

    IEnumerator C_PlayWalkSound()
    {
        if(!walkSound.isPlaying)
        {
            walkSound.Play();
        }
        
        float volume = walkSound.volume;

        while(volume < 1f)
        {
            volume += 4f*Time.deltaTime;
            walkSound.volume = volume;
            yield return null;
        }
        walkSound.volume = 1f;
    }

    public void StopWalkSound()
    {
        if(isWalking)
        {
            isWalking = false;
            if(null != walkSoundRoutine)
            {
                StopCoroutine(walkSoundRoutine);
            }
            walkSoundRoutine = C_StopWalkSound();
            StartCoroutine(walkSoundRoutine);
        }
    }
    IEnumerator C_StopWalkSound()
    {
        float volume = walkSound.volume;

        while(volume > 0f)
        {
            volume -= 2f*Time.deltaTime;
            walkSound.volume = volume;
            yield return null;
        }
        walkSound.volume = 0f;
        walkSound.Stop();
    }

    public void PlayHeartBeatSound()
    {
        if(!heartBeatSound.isPlaying)
        {
            heartBeatSound.Play();
        }
    }

    public void LoopScareSound(float vol)
    {
        scareSound.volume = vol;
        if(!scareSound.isPlaying)
        {
            scareSound.Play();
        }
    }
    public void StopLoopScareSound()
    {
        scareSound.Stop();
    }
}
