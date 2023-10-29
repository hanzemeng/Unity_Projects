using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera mainCamera;

    private void Awake()
    {
        if(null != mainCamera)
        {
            Destroy(gameObject);
            return;
        }
        mainCamera = this;
        DontDestroyOnLoad(gameObject);
    }
}
