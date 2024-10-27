using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenScaler : MonoBehaviour
{
    private CanvasScaler scaler;

    // Start is called before the first frame update
    void Start()
    {
        scaler = GetComponent<CanvasScaler>();
        float height = Screen.height;
        float width = Screen.width;
        float refResolution = 1920 / 1080;
        float currentResolution = height / width;
        scaler.scaleFactor = currentResolution / refResolution;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
