using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTrigger : ItemTrigger
{
    public GameObject halluciantionTrigger;
    public GameObject bloodTrigger;
    public GameObject bloodTrigger2;

    public GameObject boot1;
    public GameObject boot2;
    public override void PickUpTrigger()
    {
        halluciantionTrigger.SetActive(true);
        bloodTrigger.SetActive(true);
        bloodTrigger2.SetActive(true);
        boot1.SetActive(false);
        boot2.SetActive(false);
        Destroy(GetComponent<AxeTrigger>());
    }
}
