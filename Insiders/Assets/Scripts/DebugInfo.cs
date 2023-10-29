using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {
        //Debug.Log(PlayerData.hideValue);
        if(GlobalVariable.TAKING_INPUT)
        {
            Debug.Log("Reading");
        }
        else
        {
            Debug.Log("Rejecting");
        }
    }
}
