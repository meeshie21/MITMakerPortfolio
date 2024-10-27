using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeAudio()
    {
        int a = PlayerPrefs.GetInt("AudioValue");

        if (a < 4)
        {
            PlayerPrefs.SetInt("AudioValue", a + 1);
        }

        else
        {
            PlayerPrefs.SetInt("AudioValue", 1);
        }
    }
}
