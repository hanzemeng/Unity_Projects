using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterTrigger : ItemTrigger
{
    public GameObject bloodTrigger;
    public override void PickUpTrigger()
    {
        bloodTrigger.SetActive(true);
        Destroy(GetComponent<LighterTrigger>());
    }
}
