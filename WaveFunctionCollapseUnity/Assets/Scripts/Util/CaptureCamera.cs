using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCamera : MonoBehaviour // stolen from Vicente
{

    public bool EnableCapture = false;
    public float Interval = 1f;
    public int Counter = 0;

    string _folder = @"C:\Users\Kevin\Documents\WFC\ScreenCapture";

    void Start()
    {
        Time.captureFramerate = 2;
    }
    
    public void SetPath(string path)
    {
        _folder = path + @"\ScreenCapture";
    }

    void Update()
    {

        if (EnableCapture)
            StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        yield return new WaitForSeconds(Interval);

        string filename = $@"{_folder}\image_{Time.frameCount:00000}.png";

        ScreenCapture.CaptureScreenshot(filename, 1);
    }

    public void CaptureOneShot()
    {
        Counter++;
        string filename = $@"{_folder}\TestSeed_{Counter}.png";
        
        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}
