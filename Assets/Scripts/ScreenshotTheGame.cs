using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTheGame : MonoBehaviour
{
    int count = 0;
    public GameObject[] allobj;
    int run = 0;
    bool running = false;
    int tickCount;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            //Screenshot();
            running = true;
            tickCount = 0;
        }

        if (running)
        {
            if(tickCount == 0)
            {
                if (run < allobj.Length)
                {
                    allobj[run].SetActive(true);
                    Screenshot(allobj[run].name);
                }
                else running = false;
                tickCount = 1;
            }
            else if(tickCount == 1)
            {
                Debug.Log("done: " + run);
                allobj[run].SetActive(false);
                run++;
                tickCount = 0;
            }        
        }
    }

    void Screenshot(string objName)
    {
        ScreenCapture.CaptureScreenshot(objName + ".png");
        count++;
    }
}
