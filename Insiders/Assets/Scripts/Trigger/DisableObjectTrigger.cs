using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectTrigger : MonoBehaviour
{
    public GameObject targetObject;
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            targetObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
