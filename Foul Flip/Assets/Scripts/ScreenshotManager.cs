using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private static int num;

    private void Start()
    {
        num = 0;
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //DeepLinkManager.deepLinkManager.OnDeepLinkActivated("flfl://level?3&2&666");
            ScreenCapture.CaptureScreenshot($"/Users/hanzemeng/Desktop/{num}.png");
        }
    }
}
