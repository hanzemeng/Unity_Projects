using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodRevealTrigger : MonoBehaviour
{
   public Blood[] bloods;

   void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            StartCoroutine(RevealBlood());
            
        }
    }

    IEnumerator RevealBlood()
    {
        for(int i=bloods.Length-1; i>=0; i--)
        {
            bloods[i].Reveal();
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }
}
