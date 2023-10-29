using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectTrigger : MonoBehaviour
{
    public GameObject targetObject;
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            targetObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
