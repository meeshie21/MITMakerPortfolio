using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] private Fade fade;

    public void Play()
    {
        Vibrate();
        fade.LoadScene(1);
    }

    public void Menu()
    {
        Vibrate();
        fade.LoadScene(0);
    }

    public void Credits()
    {
        Vibrate();
        fade.LoadScene(2);
    }

    public void Vibrate()
    {
        Vibration.Init();
        Vibration.VibratePeek();
    }
}
