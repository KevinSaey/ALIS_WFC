using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class captureCamera : MonoBehaviour // stolen from Vicente
{

    [SerializeField]
    bool EnableCapture = false;

    string folder = @"D:\Unity\School\ALIS_WFC\WaveFunctionCollapseUnity\ScreenCapture";
    
    void Start()
    {
        Time.captureFramerate = 2;
    }
    
    void Update()
    {

        if (EnableCapture)
            StartCoroutine(Capture());
    }
    
    IEnumerator Capture()
    {
        yield return new WaitForSeconds(1f);

        string filename = $@"{folder}\image_{Time.frameCount:00000}.png";

        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}
