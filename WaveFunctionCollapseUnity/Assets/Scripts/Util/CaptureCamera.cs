using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCamera : MonoBehaviour // stolen from Vicente
{

    public bool EnableCapture = false;
    public float Interval = 1f;

    string folder = @"C:\Users\Kevin\Documents\WFC\ScreenCapture";

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
        yield return new WaitForSeconds(Interval);

        string filename = $@"{folder}\image_{Time.frameCount:00000}.png";

        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}
