using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostItYellowTrigger : ItemTrigger
{
    public LightSwitch lampLight;
    public override void PickUpTrigger()
    {
        lampLight.Flicker();
        Destroy(GetComponent<PostItYellowTrigger>());
    }
}
