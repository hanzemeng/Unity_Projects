using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenCloseTrigger : MonoBehaviour
{
    public OpenClose door;
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            if(door.objectOpen)
            {
                door.ObjectClicked();
            }
            Destroy(gameObject);
        }
    }
}
