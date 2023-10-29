using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite UISprite;
    
    public AudioSource pickUpSound;
    public AudioSource useSound;

    public void PlayPickUpSound()
    {
        pickUpSound.Play();
    }
    public void PlayUseSound()
    {
        useSound.Play();
    }
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
