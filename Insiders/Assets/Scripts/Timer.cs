using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer timer;
    private float elapsedGameTime;
    void Awake()
    {
        timer = this;
        elapsedGameTime = 0f;
    }

    public float GetElapsedGameTime()
    {
        return elapsedGameTime;
    }

    void Update()
    {
        if(GlobalVariable.TAKING_INPUT)
        {
            elapsedGameTime += Time.deltaTime;
        }
    }
}
