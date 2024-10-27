using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    new public Renderer renderer;

    public float rFloat;
    public float gFloat;
    public float bFloat;
    public float aFloat;

    void Start()
    {
        renderer = GameObject.Find("Ground").GetComponent<Renderer>();
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("ColorValue") == 1)
        {
            rFloat = 1f;
            gFloat = 0.2f;
            bFloat = 0.2f;
            aFloat = 1f;
        }

        if (PlayerPrefs.GetInt("ColorValue") == 2)
        {
            rFloat = 0.28f;
            gFloat = 0.39f;
            bFloat = 0.91f;
            aFloat = 1f;
        }

        if (PlayerPrefs.GetInt("ColorValue") == 3)
        {
            rFloat = 0.77f;
            gFloat = 0.02f;
            bFloat = 0.91f;
            aFloat = 1f;
        }

        if (PlayerPrefs.GetInt("ColorValue") == 4)
        {
            rFloat = 0.14f;
            gFloat = 0.82f;
            bFloat = 0.15f;
            aFloat = 1f;
        }

        if (PlayerPrefs.GetInt("ColorValue") == 5)
        {
            rFloat = 0.25f;
            gFloat = 0.25f;
            bFloat = 0.25f;
            aFloat = 1f;
        }

        renderer.material.color = new Color(rFloat, gFloat, bFloat, aFloat);

    }

    public void ChangeColor()
    {
        int a = PlayerPrefs.GetInt("ColorValue");
        
        if (a < 5)
        {
            PlayerPrefs.SetInt("ColorValue", a + 1);
        }

        else
        {
            PlayerPrefs.SetInt("ColorValue", 1);
        }
    }
}
