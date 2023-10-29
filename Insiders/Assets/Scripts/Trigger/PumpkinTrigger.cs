using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinTrigger : MonoBehaviour
{
    public Transform player;
    public Sprite image;

    public AudioSource appearSound;

    void OnEnable()
    {
        gameObject.transform.LookAt(player);
        Vector3 temp = gameObject.transform.rotation.eulerAngles;
        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = new Vector3(0f, temp.y, temp.z);
        gameObject.transform.rotation = newRotation;

        StartCoroutine(PlayAppearSound());
        StartCoroutine(MoveForward());
        StartCoroutine(DestroySelf(1.5f));
    }
    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        Hallucination.hallucination.Hallucinate(image);
        Destroy(gameObject);
    }
    IEnumerator MoveForward()
    {
        while(true)
        {
            gameObject.transform.position += 2f*Time.deltaTime*gameObject.transform.forward;
            yield return null;
        }
    }
    IEnumerator PlayAppearSound()
    {
        yield return new WaitForSeconds(0.15f);
        appearSound.Play();
        float volume = 1f;
        while(volume>0f)
        {
            volume -= 0.5f*Time.deltaTime;
            appearSound.volume = volume;
            yield return null;
        }
    }
}
