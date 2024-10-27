using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] private Fade fade;

    public void LoadLevelSelect()
    {
        Vibrate();
        fade.LoadScene(1); 
    }

    public void LoadMenu() 
    {
        Vibrate();
        fade.LoadScene(0); 
    }

    public void Starter()
    {
        Vibrate();
        PlayerPrefs.SetInt("difficultyLevel", -1);
        fade.LoadScene(2);
    }

    public void Easy() 
    {
        Vibrate();
        PlayerPrefs.SetInt("difficultyLevel", 0);
        fade.LoadScene(2); 
    }

    public void Normal()
    {
        Vibrate();
        PlayerPrefs.SetInt("difficultyLevel", 1);
        fade.LoadScene(2);
    }

    public void Hard()
    {
        Vibrate();
        PlayerPrefs.SetInt("difficultyLevel", 2);
        fade.LoadScene(2);
    }

    public void Death()
    {
        Vibrate();
        PlayerPrefs.SetInt("difficultyLevel", 3);
        fade.LoadScene(2);
    }

    public void LoadJumpTutorial()
    {
        Vibrate();
        PlayerPrefs.SetString("tutorial", "jump");
        fade.LoadScene(4);
    }

    public void LoadCredits()
    {
        Vibrate();
        fade.LoadScene(8);
    }

    public void Ready()
    {
        PlayerPrefs.SetString("tutorial", "done");
    }

    private void Vibrate()
    {
        Vibration.Init();
        Vibration.VibratePeek();
    }
}
