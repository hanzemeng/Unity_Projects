using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public AudioSource revealSound;
    public bool isRevealed;

    public void Reveal()
    {
        if(isRevealed)
        {
            return;
        }
        revealSound.Play();
        GetComponent<Renderer>().enabled = true;
    }
}
