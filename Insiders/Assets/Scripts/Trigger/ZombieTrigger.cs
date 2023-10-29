using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTrigger : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(DestroySelf(5f));
    }
    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
