using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallucinationTrigger : MonoBehaviour
{
    public Sprite hallucinationImage;

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            Hallucination.hallucination.Hallucinate(hallucinationImage);
            Destroy(gameObject);
        }
    }
}
