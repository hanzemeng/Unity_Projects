using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCirclePuzzle : MonoBehaviour
{
    public LightSwitch[] candles;
    int lightedCandleCount;

    public GameObject magicCicle;
    public GameObject doll;
    public GameObject tableNoiseTrigger;
    
   
    void Update()
    {
        lightedCandleCount = 0;
        for(int i=0; i<3; i++)
        {
            if(candles[i].isOn)
            {
                lightedCandleCount++;
            }
        }

        if(lightedCandleCount >= 2)
        {
            doll.SetActive(false);
        }

        if(3 == lightedCandleCount)
        {
            magicCicle.SetActive(true);
            tableNoiseTrigger.SetActive(true);
            Destroy(GetComponent<MagicCirclePuzzle>());
        }
        
    }
}
