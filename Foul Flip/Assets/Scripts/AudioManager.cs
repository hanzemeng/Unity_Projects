using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;

    private AudioSource audioSource;

    public AudioClip nodeClick;
    public AudioClip swipe;
    public AudioClip iconClick;
    public AudioClip win;

    public AudioClip keyType;

    private void Awake()
    {
        if(null != audioManager)
        {
            Destroy(gameObject);
            return;
        }
        audioManager = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNodeClick()
    {
        audioSource.PlayOneShot(nodeClick);
    }
    public void PlaySwipe()
    {
        audioSource.PlayOneShot(swipe);
    }
    public void PlayIconClick()
    {
        audioSource.PlayOneShot(iconClick);
    }
    public void PlayWin()
    {
        audioSource.PlayOneShot(win);
    }
    public void PlayKeyType()
    {
        audioSource.PlayOneShot(keyType);
    }
}
